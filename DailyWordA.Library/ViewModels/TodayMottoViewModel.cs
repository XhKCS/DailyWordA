using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;

namespace DailyWordA.Library.ViewModels;

public class TodayMottoViewModel : ViewModelBase {
    private readonly IDailyMottoService _dailyMottoService;
    
    private readonly IRootNavigationService _rootNavigationService;

    public TodayMottoViewModel(IDailyMottoService dailyMottoService, IRootNavigationService rootNavigationService) {
        _dailyMottoService = dailyMottoService;
        _rootNavigationService = rootNavigationService;
        
        OnInitializedCommand = new AsyncRelayCommand(OnInitializedAsync);
        NavigateToResultViewCommand = new RelayCommand(NavigateToResultView);
    }
    
    private DailyMotto _todayMotto;
    public DailyMotto TodayMotto {
        get => _todayMotto;
        set => SetProperty(ref _todayMotto, value);
    }
    
    private bool _isLoading;
    public bool IsLoading {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }
    
    public ICommand OnInitializedCommand { get; }

    public async Task OnInitializedAsync() {
        IsLoading = true;
        TodayMotto = await _dailyMottoService.GetTodayMottoAsync();
        IsLoading = false;
    }
    
    public ICommand NavigateToResultViewCommand { get; }

    private void NavigateToResultView() {
        _rootNavigationService.NavigateTo(nameof(WordResultViewModel));
    }
}