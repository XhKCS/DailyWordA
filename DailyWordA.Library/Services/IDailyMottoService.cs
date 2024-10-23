using DailyWordA.Library.Models;

namespace DailyWordA.Library.Services;

// 目前只用一种来源，就是api
public interface IDailyMottoService {
    Task<DailyMotto> GetTodayMottoAsync();
}