using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;

namespace DailyWordA.Services;

public class RootNavigationService : IRootNavigationService {
    public void NavigateTo(string view) {
        // 在这里给MainWindowViewModel中的Content赋值
        if (view == RootNavigationConstant.MainView) {
            ServiceLocator.Current.MainWindowViewModel.Content = 
                ServiceLocator.Current.MainViewModel;
            ServiceLocator.Current.MainViewModel.SetMenuAndContent(
                MenuNavigationConstant.TodayWordView, ServiceLocator.Current.TodayWordViewModel);
        }
        else if (view == RootNavigationConstant.InitializationView) {
            ServiceLocator.Current.MainWindowViewModel.Content = 
                ServiceLocator.Current.InitializationViewModel;
        }
        
    }
}