using DailyWordA.Library.Models;
using DailyWordA.Library.Services;

namespace DailyWordA.Library.ViewModels;

public class MottoDetailViewModel : ViewModelBase {
    private readonly IMenuNavigationService _menuNavigationService;

    public MottoDetailViewModel(IMenuNavigationService menuNavigationService) {
        _menuNavigationService = menuNavigationService;
    }
    
    private DailyMotto _todayMotto;
    public DailyMotto TodayMotto {
        get => _todayMotto;
        private set => SetProperty(ref _todayMotto, value);
    }
    
    public bool CanShowSource => _todayMotto.Source.Length > 0;
    
    public bool CanShowAuthor => _todayMotto.Author.Length > 0;
    
    public override void SetParameter(object parameter) {
        TodayMotto = parameter as DailyMotto;
    }

}