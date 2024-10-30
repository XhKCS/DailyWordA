using DailyWordA.Library.Models;

namespace DailyWordA.Library.Services;

public interface ITodayImageService {
    Task<TodayImage> GetTodayImageAsync(); // 获取当日的图片
    
    Task<TodayImageServiceCheckUpdateResult> CheckUpdateAsync();
    
    Task<TodayImage> GetRandomImageAsync(); // 获取随机图片，不限于当日
}

public class TodayImageServiceCheckUpdateResult {
    public bool HasUpdate { get; set; }

    public TodayImage TodayImage { get; set; } = new();
}