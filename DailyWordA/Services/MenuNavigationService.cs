using System;
using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;

namespace DailyWordA.Services;

public class MenuNavigationService : IMenuNavigationService {
    public void NavigateTo(string view, object parameter = null) {
        ViewModelBase viewModel = view switch {
            MenuNavigationConstant.TodayWordView => ServiceLocator.Current
                .TodayWordViewModel,
            MenuNavigationConstant.TodayMottoView => ServiceLocator.Current
                .TodayMottoViewModel,
            MenuNavigationConstant.TranslateView => ServiceLocator.Current.
                TranslateViewModel,
            MenuNavigationConstant.WordQueryView => ServiceLocator.Current.
                WordQueryViewModel,
            MenuNavigationConstant.WordFavoriteView => ServiceLocator.Current.
                WordFavoriteViewModel,
            MenuNavigationConstant.WordSelectionView => ServiceLocator.Current.
                WordSelectionViewModel,
            MenuNavigationConstant.WordDictationView => ServiceLocator.Current.
                WordDictationViewModel,
            MenuNavigationConstant.WordMistakeNoteView => ServiceLocator.Current.
                WordMistakeNoteViewModel,
            _ => throw new Exception("未知的视图。")
        };

        if (parameter is not null) {
            viewModel.SetParameter(parameter);
        }

        ServiceLocator.Current.MainViewModel.SetMenuAndContent(view, viewModel);
    }
}