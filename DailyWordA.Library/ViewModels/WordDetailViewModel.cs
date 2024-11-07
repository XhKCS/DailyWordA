using DailyWordA.Library.Models;
using DailyWordA.Library.Services;

namespace DailyWordA.Library.ViewModels;

public class WordDetailViewModel : ViewModelBase {
    private readonly IMenuNavigationService _menuNavigationService;

    public WordDetailViewModel(IMenuNavigationService menuNavigationService) {
        _menuNavigationService = menuNavigationService;
    }
    
    private WordObject _todayWord;
    public WordObject TodayWord {
        get => _todayWord;
        set => SetProperty(ref _todayWord, value);
    }

    public bool CanShowPhrase => _todayWord.Phrase.Length > 0;
    
    public bool CanShowEtyma => _todayWord.Etyma.Length > 0;

    public override void SetParameter(object parameter) {
        TodayWord = parameter as WordObject;
    }
    
}