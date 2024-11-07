namespace DailyWordA.Library.Services;

public interface IContentNavigationService {
    void NavigateTo(string view, object parameter = null);
}

public static class ContentNavigationConstant {
    public const string WordDetailView = nameof(WordDetailView);
    
    public const string MottoDetailView = nameof(MottoDetailView);

    public const string ResultView = nameof(ResultView);

    public const string DetailView = nameof(DetailView);
}