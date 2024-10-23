using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;

namespace DailyWordA.Services;

public class RootNavigationService : IRootNavigationService {
    public void NavigateTo(string view) {
        // 在这里给MainWindowViewModel中的Content赋值
        if (view == nameof(RootNavigationConstant.MainView)) {
            ServiceLocator.Current.MainWindowViewModel.Content = 
                ServiceLocator.Current.MainViewModel;
            ServiceLocator.Current.MainViewModel.PushContent(ServiceLocator.Current.WordResultViewModel);
        }
        else if (view == nameof(WordResultViewModel)) {
            ServiceLocator.Current.MainWindowViewModel.Content = 
                ServiceLocator.Current.MainViewModel;
            ServiceLocator.Current.MainViewModel.SetMenuAndContent(MenuNavigationConstant.TodayWordView, ServiceLocator.Current.WordResultViewModel);
        } 
        else if (view == nameof(TodayMottoViewModel)) {
            ServiceLocator.Current.MainWindowViewModel.Content = 
                ServiceLocator.Current.MainViewModel;
            ServiceLocator.Current.MainViewModel.SetMenuAndContent(MenuNavigationConstant.TodayMottoView, ServiceLocator.Current.TodayMottoViewModel);
        }
        
    }
}