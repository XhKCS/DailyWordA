using DailyWordA.Library.Models;

namespace DailyWordA.Library.Services;

public interface ITodayImageStorage {
    Task<TodayImage> GetTodayImageAsync(bool isIncludingImageStream);

    Task SaveTodayImageAsync(TodayImage todayImage, bool isSavingExpiresAtOnly);
    
    Task TrySaveDefaultImageAsync();
}