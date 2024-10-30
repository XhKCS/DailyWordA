namespace DailyWordA.Library.Models;

public class TodayImage {
    public string StartDate { get; set; } = string.Empty; // 为了方便，就用yyyyMMdd格式的8位字符串

    public DateTime ExpiresAt { get; set; }
    
    public string Copyright { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public byte[] ImageBytes { get; set; }
}