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
    
    private readonly Random _random = new();
    
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
    
    public async Task<string> Translate(string sourceText, string from="auto", string to="zh") {
        var q = sourceText;
        var salt = _random.Next(1000, 10000).ToString();
        var sign = GetSign(q, salt);
        
        // Console.WriteLine("1");
        const string server = "百度翻译服务器";
        using var httpClient = new HttpClient();
        HttpResponseMessage response;
        try {
            // Console.WriteLine("2");
            response =
                await httpClient.GetAsync(
                    $"http://api.fanyi.baidu.com/api/trans/vip/translate?q={q}&from={from}&to={to}" +
                    $"&appid={GetOriginal1(BaiduArgs.GetId())}&salt={salt}&sign={sign}");
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
    
    private static string GetOriginal1(string strPath) {
        string returnData;
        byte[] bpath = Convert.FromBase64String(strPath);
        try {
            returnData = Encoding.UTF8.GetString(bpath);
        }
        catch {
            returnData = strPath;
        }
        
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < returnData.Length; i++) {
            int x = int.Parse(returnData.Substring(i, 1));
            x -= i;
            while (x < 0) {
                x += 10;
            }
            sb.Append(x);
        }
        return sb.ToString();
    }

    private static string GetSign(string q, string salt) {
        string originalSign = GetOriginal1(BaiduArgs.GetId()) + q + salt + GetOriginal2(BaiduArgs.GetKey());
        return Md5Crypto(originalSign);
    }
    
    private static string GetOriginal2(string strPath) {
        string returnData;
        byte[] bpath = Convert.FromBase64String(strPath);
        try {
            returnData = Encoding.UTF8.GetString(bpath);
        }
        catch {
            returnData = strPath;
        }

        var chars = returnData.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
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

public static class BaiduArgs {
    public static string GetId() {
        return "MjE0NzU1ODk4OTIyMDUyMDE=";
    }

    public static string GetKey() {
        return "YVZRbWxwdk5HVGF2czJ1TWp2NFc=";
    }
}