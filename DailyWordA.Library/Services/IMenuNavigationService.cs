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
    
    public const string WordSelectionView = nameof(WordSelectionView); //单词测验-中英选择
    
    public const string WordDictationView = nameof(WordDictationView); //单词测验-听音写词
    
    public const string WordMistakeNoteView = nameof(WordMistakeNoteView); //单词错题本
}