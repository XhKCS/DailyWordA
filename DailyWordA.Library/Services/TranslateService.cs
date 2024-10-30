using System.Text.Json;
using DailyWordA.Library.Helpers;

namespace DailyWordA.Library.Services;

public class TranslateService : ITranslateService {
    private readonly IAlertService _alertService;

    public TranslateService(IAlertService alertService) {
        _alertService = alertService;
    }
    
    public async Task<string> Translate(string sourceText) {
        Console.WriteLine("1");
        const string server = "夏柔谷歌翻译服务器";
        using var httpClient = new HttpClient();
        HttpResponseMessage response;
        try {
            Console.WriteLine("2");
            response =
                await httpClient.GetAsync(
                    $"https://findmyip.net/api/translate.php?text={sourceText}");
            // response.EnsureSuccessStatusCode();
            Console.WriteLine("3");
        } catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.HttpClientErrorTitle,
                ErrorMessageHelper.GetHttpClientError(server, e.Message));
            return string.Empty;
        }
        
        var json = await response.Content.ReadAsStringAsync();
        TranslateResponse translateResponse;
        try {
            translateResponse = JsonSerializer.Deserialize<TranslateResponse>(
                json,
                new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new JsonException();
            Console.WriteLine("4");
        } catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.JsonDeserializationErrorTitle,
                ErrorMessageHelper.GetJsonDeserializationError(server,
                    e.Message));
            return string.Empty;
        }

        return translateResponse.Data.translate_result.Replace("&#39;", "'");
    }
}

// 翻译API的返回格式
public class TranslateResponse {
    public int Code { get; set; }
    public ResponseData Data { get; set; }
    public string ProcessTime { get; set; }
    public string Url { get; set; }
    public string Time { get; set; }
}
public class ResponseData {
    public string source_lang { get; set; }
    public string target_lang { get; set; }
    public string translate_result { get; set; }
}