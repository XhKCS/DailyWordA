using System.Net.Http.Headers;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using DailyWordA.Library.Helpers;

namespace DailyWordA.Library.Services;

public class TranslateService : ITranslateService {
    private readonly IAlertService _alertService;

    public TranslateService(IAlertService alertService) {
        _alertService = alertService;
    }
    
    private readonly Random _random = new Random();
    
    // MD5加密为32位字符串
    private static string Md5Crypto(string plainText) {
        var md5 = MD5.Create();
        string sign = "";
        
        byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(plainText));
        for (int i = 0; i < s.Length; i++) {
            sign += s[i].ToString("x2");
        }
        return sign;
    }
    
    // 使用百度翻译API
    public async Task<string> Translate(string sourceText, string from="auto", string to="zh") {
        string q = sourceText;
        string salt = _random.Next(1000, 10000).ToString();
        string originalSign = BaiduTranslateArgs.Appid + q + salt + BaiduTranslateArgs.SecretKey;
        string sign = Md5Crypto(originalSign);
        
        // Console.WriteLine("1");
        const string server = "百度翻译服务器";
        using var httpClient = new HttpClient();
        HttpResponseMessage response;
        try {
            // Console.WriteLine("2");
            response =
                await httpClient.GetAsync(
                    $"http://api.fanyi.baidu.com/api/trans/vip/translate?q={q}&from={from}&to={to}" +
                    $"&appid={BaiduTranslateArgs.Appid}&salt={salt}&sign={sign}");
            response.EnsureSuccessStatusCode();
            // Console.WriteLine("3");
        } catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.HttpClientErrorTitle,
                ErrorMessageHelper.GetHttpClientError(server, e.Message));
            return string.Empty;
        }
        
        var json = await response.Content.ReadAsStringAsync();
        BaiduTranslateResponse baiduTranslateResponse;
        try {
            baiduTranslateResponse = JsonSerializer.Deserialize<BaiduTranslateResponse>(
                json,
                new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new JsonException();
            // Console.WriteLine("4");
        } catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.JsonDeserializationErrorTitle,
                ErrorMessageHelper.GetJsonDeserializationError(server,
                    e.Message));
            return string.Empty;
        }
        
        return baiduTranslateResponse.trans_result[0].Dst;
    }
    
}

// 翻译API的返回格式
public class BaiduTranslateResponse {
    public string From { get; set; }
    public string To { get; set; }
    public BaiduTransResult[] trans_result { get; set; }
}
public class BaiduTransResult {
    public string Src { get; set; }
    public string Dst { get; set; }
}

public class BaiduTranslateArgs {
    public const string Appid = "20241022002182855";
    public const string SecretKey = "W4vjMu2svaTGNvplmQVa";
}