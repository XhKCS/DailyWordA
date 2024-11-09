using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;

namespace DailyWordA.Library.ViewModels;

public class TodayMottoViewModel : ViewModelBase {
    private readonly IDailyMottoService _dailyMottoService;
    private readonly ITodayImageService _todayImageService;
    private readonly IContentNavigationService _contentNavigationService;
    
    private readonly IMenuNavigationService _menuNavigationService;

    public TodayMottoViewModel(IDailyMottoService dailyMottoService, 
        ITodayImageService todayImageService,
        IContentNavigationService contentNavigationService,
        IMenuNavigationService menuNavigationService) {
        _dailyMottoService = dailyMottoService;
        _todayImageService = todayImageService;
        _contentNavigationService = contentNavigationService;
        _menuNavigationService = menuNavigationService;
        
        UpdateAsync();
        
        UpdateCommand = new RelayCommand(UpdateAsync);
        ShowDetailCommand = new RelayCommand(ShowDetail);
        NavigateToResultViewCommand = new RelayCommand(NavigateToTodayWordView);
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
    public void UpdateAsync() {
        Task.Run(async () => {
            IsLoading = true;
            TodayImage = await _todayImageService.GetRandomImageAsync();
            IsLoading = false;
        });

        Task.Run(async () => {
            TodayMotto = await _dailyMottoService.GetTodayMottoAsync();
        });
    }
    
    // 跳转至格言详情页
    public ICommand ShowDetailCommand { get; }
    private void ShowDetail() {
        // 跳转至详情页面，注意要传参：当前的TodayMotto
        _contentNavigationService.NavigateTo(
            ContentNavigationConstant.MottoDetailView, TodayMotto);
    }
    
    public ICommand NavigateToResultViewCommand { get; }
    private void NavigateToTodayWordView() {
        _menuNavigationService.NavigateTo(MenuNavigationConstant.TodayWordView);
    }
}