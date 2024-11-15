namespace DailyWordA.Library.Services;

public interface IMenuNavigationService {
    void NavigateTo(string view, object parameter = null);
}

public static class MenuNavigationConstant {
    public const string TodayWordView = nameof(TodayWordView);

    public const string TodayMottoView = nameof(TodayMottoView);
    
    public const string WordQueryView = nameof(WordQueryView);

    public const string WordFavoriteView = nameof(WordFavoriteView);
    
    // public const string MottoFavoriteView = nameof(MottoFavoriteView);
    
    public const string TranslateView = nameof(TranslateView);
    
    public const string WordQuizView = nameof(WordQuizView); //单词测验
}