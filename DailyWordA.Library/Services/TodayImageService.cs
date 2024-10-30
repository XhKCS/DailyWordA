using System.Globalization;
using System.Text.Json;
using DailyWordA.Library.Helpers;
using DailyWordA.Library.Models;

namespace DailyWordA.Library.Services;

public class TodayImageService : ITodayImageService{
    private readonly IAlertService _alertService;
    private readonly ITodayImageStorage _todayImageStorage;
    
    private static HttpClient _httpClient = new();

    public TodayImageService(IAlertService alertService,
        ITodayImageStorage todayImageStorage) {
        _alertService = alertService;
        _todayImageStorage = todayImageStorage;
    }
    
    public async Task<TodayImage> GetTodayImageAsync() {
        return await _todayImageStorage.GetTodayImageAsync(true);
    }

    public async Task<TodayImageServiceCheckUpdateResult> CheckUpdateAsync() {
        var todayImage = await _todayImageStorage.GetTodayImageAsync(false);
        if (todayImage.ExpiresAt > DateTime.Now) {
            // 还没到过期时间，暂时还不用更新
            return new TodayImageServiceCheckUpdateResult { HasUpdate = false };
        }
        
        const string server = "必应每日图片服务器";
        HttpResponseMessage response;
        try {
            response = await _httpClient.GetAsync(
                "https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=zh-CN");
            response.EnsureSuccessStatusCode();
        } catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.HttpClientErrorTitle,
                ErrorMessageHelper.GetHttpClientError(server, e.Message));
            return new TodayImageServiceCheckUpdateResult { HasUpdate = false };
        }

        var json = await response.Content.ReadAsStringAsync();
        string bingImageUrl;
        try {
            var bingImage = JsonSerializer.Deserialize<BingImageOfTheDay>(json,
                new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                })?.Images?.FirstOrDefault() ?? throw new JsonException();

            var bingImageStartDate = DateTime.ParseExact(
                bingImage.StartDate ?? throw new JsonException(),
                "yyyyMMdd", CultureInfo.InvariantCulture);
            var todayImageStartDate = DateTime.ParseExact(
                todayImage.StartDate, "yyyyMMdd",
                CultureInfo.InvariantCulture);
            
            if (bingImageStartDate < todayImageStartDate) {
                // 说明当前Bing图片还没有更新，那就把当前图片的过期时间再加两小时
                todayImage.ExpiresAt = DateTime.Now.AddHours(2);
                await _todayImageStorage.SaveTodayImageAsync(todayImage, true);
                return new TodayImageServiceCheckUpdateResult {
                    HasUpdate = false
                };
            }
            // 需要更新
            todayImage = new TodayImage {
                StartDate = bingImage.StartDate,
                ExpiresAt = bingImageStartDate.AddDays(1),
                Copyright = bingImage.Copyright ?? throw new JsonException(),
                Title = bingImage.Title ?? throw new JsonException()
            };

            bingImageUrl = bingImage.Url ?? throw new JsonException();
        } catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.JsonDeserializationErrorTitle,
                ErrorMessageHelper.GetJsonDeserializationError(server,
                    e.Message));
            return new TodayImageServiceCheckUpdateResult { HasUpdate = false };
        }

        try {
            response =
                await _httpClient.GetAsync(
                    "https://www.bing.com" + bingImageUrl);
            response.EnsureSuccessStatusCode();
        } catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.HttpClientErrorTitle,
                ErrorMessageHelper.GetHttpClientError(server, e.Message));
            return new TodayImageServiceCheckUpdateResult { HasUpdate = false };
        }
        // 更新todayImage，保存并返回
        todayImage.ImageBytes = await response.Content.ReadAsByteArrayAsync();
        await _todayImageStorage.SaveTodayImageAsync(todayImage, false);
        return new TodayImageServiceCheckUpdateResult {
            HasUpdate = true, TodayImage = todayImage
        };
    }

    public async Task<TodayImage> GetRandomImageAsync() {
        const string server = "必应随机图片服务器";
        TodayImage randomImage;
        HttpResponseMessage response;
        try {
            response = await _httpClient.GetAsync(
                "https://api.xsot.cn/bing/?mkt=zh-cn");
            response.EnsureSuccessStatusCode();
        } catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.HttpClientErrorTitle,
                ErrorMessageHelper.GetHttpClientError(server, e.Message));
            return null;
        }
        var json = await response.Content.ReadAsStringAsync();
        string bingImageFullUrl;
        try
        {
            var bingRandomImage = JsonSerializer.Deserialize<BingRandomImageResponse>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }).Data;

            randomImage = new TodayImage {
                StartDate = bingRandomImage.Date.ToString(),
                Title = bingRandomImage.Title
            };
            bingImageFullUrl = bingRandomImage.Image ?? throw new JsonException();
        } catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.JsonDeserializationErrorTitle,
                ErrorMessageHelper.GetJsonDeserializationError(server, 
                    e.Message));
            return null;
        }
        try {
            response =
                await _httpClient.GetAsync(bingImageFullUrl);
            response.EnsureSuccessStatusCode();
        } catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.HttpClientErrorTitle,
                ErrorMessageHelper.GetHttpClientError(server, e.Message));
            return null;
        }

        randomImage.ImageBytes = await response.Content.ReadAsByteArrayAsync();
        return randomImage;
    }
}

// Bing今日图片的返回格式
public class BingImageOfTheDay {
    public List<BingImageOfTheDayImage> Images { get; set; }
}
public class BingImageOfTheDayImage {
    public string StartDate { get; set; }

    public string Url { get; set; }

    public string Copyright { get; set; }

    public string Title { get; set; }
}

// Bing随机图片api的返回格式
public class BingRandomImageResponse
{
    public BingRandomImageData Data { get; set; }
    public string Message { get; set; }
    public bool Success { get; set; }
}
public class BingRandomImageData
{
    public int Date { get; set; }
    public int Id { get; set; }
    public string Image { get; set; }
    public string Mkt { get; set; }
    public string Title { get; set; }
}