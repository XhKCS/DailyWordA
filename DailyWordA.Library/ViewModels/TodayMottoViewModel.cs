using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;

namespace DailyWordA.Library.ViewModels;

public class TodayMottoViewModel : ViewModelBase {
    private readonly IDailyMottoService _dailyMottoService;
    private readonly ITodayImageService _todayImageService;
    
    private readonly IRootNavigationService _rootNavigationService;

    public TodayMottoViewModel(IDailyMottoService dailyMottoService, 
        ITodayImageService todayImageService,
        IRootNavigationService rootNavigationService) {
        _dailyMottoService = dailyMottoService;
        _todayImageService = todayImageService;
        _rootNavigationService = rootNavigationService;
        
        UpdateAsync();
        
        UpdateCommand = new AsyncRelayCommand(UpdateAsync);
        NavigateToResultViewCommand = new RelayCommand(NavigateToResultView);
    }
    
    private DailyMotto _todayMotto;
    public DailyMotto TodayMotto {
        get => _todayMotto;
        set => SetProperty(ref _todayMotto, value);
    }
    
    private TodayImage _todayImage;
    public TodayImage TodayImage {
        get => _todayImage;
        private set => SetProperty(ref _todayImage, value);
    }
    
    private bool _isLoading;
    public bool IsLoading {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }
    
    public ICommand UpdateCommand { get; }

    public async Task UpdateAsync() {
        IsLoading = true;
        // TodayImage = await _todayImageService.GetTodayImageAsync(); //先读本地的
        TodayImage = await _todayImageService.GetRandomImageAsync(); //再更新
        
        TodayMotto = await _dailyMottoService.GetTodayMottoAsync();
        // await Task.Delay(100);
        IsLoading = false;
    }
    
    public ICommand NavigateToResultViewCommand { get; }

    private void NavigateToResultView() {
        _rootNavigationService.NavigateTo(nameof(WordResultViewModel));
    }
}