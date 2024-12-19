using System.Text;
using System.Text.Json;
using DailyWordA.Library.Helpers;
using DailyWordA.Library.Models;

namespace DailyWordA.Library.Services;

public class DailyMottoService : IDailyMottoService {
    private readonly IAlertService _alertService;
    private readonly ITranslateService _translateService;
    
    private string _domainName;

    private string _apiKey;

    public DailyMottoService(IAlertService alertService,
        ITranslateService translateService,
        string domainName = "apis.juhe.cn/fapigx/everyday/query", 
        string apiKey = "OWI3NjIxOGQxYjNmYzAwY2NiM2E5Nzg5OWYyMDY2ZDY=") {
        _alertService = alertService;
        _translateService = translateService;
        
        _domainName = domainName;
        _apiKey = apiKey;
    }

    private static string Decode(string base64EncodedData) {
        var bytes = Convert.FromBase64String(base64EncodedData);
        return Encoding.UTF8.GetString(bytes);
    }
    
    public async Task<DailyMotto> GetTodayMottoAsync()
    {
        DailyMotto todayMotto = await MeiriyingyuMottoAsync() ??
                                await HitokotoMottoAsync() ??
                                await ShanbaydanciMottoAsync();
        
        return todayMotto ?? new DailyMotto {
            Content = string.Empty,
            Translation = "非常抱歉，今日该接口似乎出现了问题..."
        };
    }

    private async Task<DailyMotto> MeiriyingyuMottoAsync() {
        const string server = "每日英语服务器";
        using var httpClient = new HttpClient();
        HttpResponseMessage response;
        try {
            response =
                await httpClient.GetAsync(
                    $"http://{_domainName}?key={Decode(_apiKey)}");
            response.EnsureSuccessStatusCode();
        } catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.HttpClientErrorTitle,
                ErrorMessageHelper.GetHttpClientError(server, e.Message));
            return null;
        }
        
        var json = await response.Content.ReadAsStringAsync();
        MeiriyingyuResponse mottoResponse;
        try {
            mottoResponse = JsonSerializer.Deserialize<MeiriyingyuResponse>(
                json,
                new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new JsonException();
        } catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.JsonDeserializationErrorTitle,
                ErrorMessageHelper.GetJsonDeserializationError(server,
                    e.Message));
            return null;
        }

        var mottoResult = mottoResponse.Result;
        if (mottoResult == null) {
            return null;
        }
        return new DailyMotto {
            Content = mottoResult.Content,
            Translation = mottoResult.Note,
            Source = mottoResult.Source,
            Date = mottoResult.Date
        };
    }

    private async Task<DailyMotto> ShanbaydanciMottoAsync() {
        const string server = "扇贝单词服务器";
        using var httpClient = new HttpClient();
        HttpResponseMessage response;
        try {
            response =
                await httpClient.GetAsync(
                    "https://apiv3.shanbay.com/weapps/dailyquote/quote");
            response.EnsureSuccessStatusCode();
        } catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.HttpClientErrorTitle,
                ErrorMessageHelper.GetHttpClientError(server, e.Message));
            return null;
        }
        
        var json = await response.Content.ReadAsStringAsync();
        ShanbaydanciResponse shanbaydanciResponse;
        try {
            shanbaydanciResponse = JsonSerializer.Deserialize<ShanbaydanciResponse>(
                json,
                new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new JsonException();
        } catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.JsonDeserializationErrorTitle,
                ErrorMessageHelper.GetJsonDeserializationError(server,
                    e.Message));
            return null;
        }
        
        return new DailyMotto {
            Content = shanbaydanciResponse.Content,
            Translation = shanbaydanciResponse.Translation,
            Date = shanbaydanciResponse.assign_date,
            Author = shanbaydanciResponse.Author
        };
    }

    private async Task<DailyMotto> HitokotoMottoAsync() {
        const string server = "hitokoto服务器";
        using var httpClient = new HttpClient();
        HttpResponseMessage response;
        try {
            response =
                await httpClient.GetAsync(
                    "https://v1.hitokoto.cn/");
            response.EnsureSuccessStatusCode();
        } catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.HttpClientErrorTitle,
                ErrorMessageHelper.GetHttpClientError(server, e.Message));
            return null;
        }
        
        var json = await response.Content.ReadAsStringAsync();
        // Console.WriteLine(json);
        HitokotoResponse hitokotoResponse;
        try {
            hitokotoResponse = JsonSerializer.Deserialize<HitokotoResponse>(
                json,
                new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new JsonException();
        } catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.JsonDeserializationErrorTitle,
                ErrorMessageHelper.GetJsonDeserializationError(server,
                    e.Message));
            return null;
        }
    
        var englishContent = await _translateService.Translate(hitokotoResponse.Hitokoto, "auto", "en");
        
        // Console.WriteLine(englishContent);
        return new DailyMotto {
            Content = englishContent,
            Translation = hitokotoResponse.Hitokoto,
            Date = GetDateStr(),
            Source = hitokotoResponse.From,
            Author = hitokotoResponse.from_who
        };
    }
    
    private async Task<DailyMotto> OneMottoAsync() {
        const string server = "One服务器";
        using var httpClient = new HttpClient();
        HttpResponseMessage response;
        try {
            response =
                await httpClient.GetAsync(
                    "https://api.xygeng.cn/one");
            response.EnsureSuccessStatusCode();
        } catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.HttpClientErrorTitle,
                ErrorMessageHelper.GetHttpClientError(server, e.Message));
            return null;
        }
        
        var json = await response.Content.ReadAsStringAsync();
        // Console.WriteLine(json);
        OneApiResponse oneApiResponse;
        try {
            oneApiResponse = JsonSerializer.Deserialize<OneApiResponse>(
                json,
                new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new JsonException();
        } catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.JsonDeserializationErrorTitle,
                ErrorMessageHelper.GetJsonDeserializationError(server,
                    e.Message));
            return null;
        }
    
        var englishContent = await _translateService.Translate(oneApiResponse.Data.content, "auto", "en");
        
        // Console.WriteLine(englishContent);
        return new DailyMotto {
            Content = englishContent,
            Translation = oneApiResponse.Data.content,
            Date = GetDateStr(),
            Source = oneApiResponse.Data.origin
        };
    }

    private static string GetDateStr()
    {
        return DateTime.Now.ToString("yyyy-MM-dd");
    }
}

// 每日英语API的返回对象类；该API优势在于同一天多次请求返回的内容并不会重复，缺点是每天限制使用50次
public class MeiriyingyuResponse {
    public MeiriyingyuResult Result { get; set; }
}
public class MeiriyingyuResult {
    public int Id { get; set; }
    public string Content { get; set; }
    public string Note { get; set; }
    public string Source { get; set; }
    public string Date { get; set; }
}

// 扇贝单词API的返回对象类；该API优势在于没有限制使用次数，缺点是同一天的请求只会返回相同的结果
public class ShanbaydanciResponse {
    public string Content { get; set; }
    public string Author { get; set; }
    public string assign_date { get; set; }
    public string Translation { get; set; }
}

public class HitokotoResponse {
    public int Id { get; set; }
    public string Uuid { get; set; }
    public string Hitokoto { get; set; }
    public string Type { get; set; }
    public string From { get; set; }
    public string from_who { get; set; }
}

public class OneApiResponse
{
    public int Code { get; set; }
    public OneApiData Data { get; set; }
    public object Error { get; set; }
    public long UpdateTime { get; set; }
}

public class OneApiData
{
    public int id { get; set; }
    public string tag { get; set; }
    public string name { get; set; }
    public string origin { get; set; }
    public string content { get; set; }
    public string created_at { get; set; }
    public string updated_at { get; set; }
}






