namespace DailyWordA.Library.Services;

public interface IRootNavigationService {
    void NavigateTo(string view);
}

public static class RootNavigationConstant {
    // 初始化数据库的页面（有旋转进度条的白底页面）
    public const string InitializationView = nameof(InitializationView);
    // 真正的外层主页面；导航只会改变主页面中的控件，也就是第二层页面
    public const string MainView = nameof(MainView);
}