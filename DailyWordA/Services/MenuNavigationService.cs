using System;
using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;

namespace DailyWordA.Services;

public class MenuNavigationService : IMenuNavigationService {
    public void NavigateTo(string view, object parameter = null) {
        ViewModelBase viewModel = view switch {
            MenuNavigationConstant.TodayWordView => ServiceLocator.Current
                .WordResultViewModel,
            MenuNavigationConstant.TodayMottoView => ServiceLocator.Current
                .TodayMottoViewModel,
            MenuNavigationConstant.TranslateView => ServiceLocator.Current.
                TranslateViewModel,
            _ => throw new Exception("未知的视图。")
        };

        // if (parameter is not null) {
        //     viewModel.SetParameter(parameter);
        // }

        ServiceLocator.Current.MainViewModel.SetMenuAndContent(view, viewModel);
    }
}