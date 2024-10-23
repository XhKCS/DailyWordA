namespace DailyWordA.Library.Services;

public interface IMenuNavigationService {
    void NavigateTo(string view, object parameter = null);
}

public static class MenuNavigationConstant {
    public const string TodayWordView = nameof(TodayWordView);

    public const string TodayMottoView = nameof(TodayMottoView);

    public const string FavoriteWordView = nameof(FavoriteWordView);
    
    public const string FavoriteMottoView = nameof(FavoriteMottoView);
    
    public const string TranslateView = nameof(TranslateView);
}