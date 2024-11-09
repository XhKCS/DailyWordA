using System;
using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;

namespace DailyWordA.Services;

public class ContentNavigationService : IContentNavigationService {
    public void NavigateTo(string view, object parameter = null) {
        ViewModelBase viewModel = view switch {
            ContentNavigationConstant.WordDetailView => ServiceLocator.Current
                .WordDetailViewModel,
            ContentNavigationConstant.MottoDetailView => ServiceLocator.Current
                .MottoDetailViewModel,
            ContentNavigationConstant.WordQueryResultView => ServiceLocator.Current
                .WordQueryResultViewModel,
            _ => throw new Exception("未知的视图。")
        };
        
        if (parameter != null) {
            viewModel.SetParameter(parameter);
        }
        
        ServiceLocator.Current.MainViewModel.PushContent(viewModel);
    }
}