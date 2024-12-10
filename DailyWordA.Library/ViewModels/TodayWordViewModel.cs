using System.Linq.Expressions;
using System.Windows.Input;
using AvaloniaInfiniteScrolling;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;

namespace DailyWordA.Library.ViewModels;

public class TodayWordViewModel : ViewModelBase {
    private readonly IWordStorage _wordStorage;
    private readonly ITodayImageService _todayImageService;
    private readonly IContentNavigationService _contentNavigationService;
    private readonly IAudioPlayer _audioPlayer;
    
    private readonly IMenuNavigationService _menuNavigationService;
    
    public  TodayWordViewModel(IWordStorage wordStorage, 
        ITodayImageService todayImageService,
        IContentNavigationService contentNavigationService,
        IMenuNavigationService menuNavigationService,
        IAudioPlayer audioPlayer) {
        _wordStorage = wordStorage;
        _todayImageService = todayImageService;
        _contentNavigationService = contentNavigationService;
        _menuNavigationService = menuNavigationService;
        _audioPlayer = audioPlayer;
        
        // _wordStorage.InitializeAsync();
        
        // OnInitializedCommand = new RelayCommand(OnInitialized);
        OnInitialized();
        UpdateWordCommand = new AsyncRelayCommand(UpdateWordAsync);
        ShowDetailCommand = new RelayCommand(ShowDetail);
        NavigateToTodayMottoViewCommand = new RelayCommand(NavigateToTodayMottoView);
        PlayAudioCommand = new AsyncRelayCommand(PlayAudio);
        
        // _wordStorage.InitializeAsyncForFirstTime();  //测试用
    }

    // 今日推荐单词
    private WordObject _todayWord;
    public WordObject TodayWord {
        get => _todayWord;
        set => SetProperty(ref _todayWord, value);
    }
    
    // 背景图片
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
    
    // public ICommand OnInitializedCommand { get; }
    // 页面初始化方法
    public void OnInitialized() {
        Task.Run(async () => {
            TodayImage = await _todayImageService.GetTodayImageAsync();
            
            var updateResult = await _todayImageService.CheckUpdateAsync();
            if (updateResult.HasUpdate) {
                TodayImage = updateResult.TodayImage;
            }
        });

        Task.Run(async () => {
            IsLoading = true;
            await Task.Delay(200);
            TodayWord = await _wordStorage.GetRandomWordAsync();
            IsLoading = false;
        });
    }
    
    // 切换单词
    public ICommand UpdateWordCommand { get; }
    public async Task UpdateWordAsync() {
        IsLoading = true;
        TodayWord = await _wordStorage.GetRandomWordAsync();
        await Task.Delay(300);
        IsLoading = false;
    }
    
    // 跳转至单词详情页
    public ICommand ShowDetailCommand { get; }
    public void ShowDetail() {
        // 跳转至详情页面，注意要传参：当前的TodayWord
        _contentNavigationService.NavigateTo(
            ContentNavigationConstant.WordDetailView, TodayWord);
    }
    
    public ICommand NavigateToTodayMottoViewCommand { get; }
    public void NavigateToTodayMottoView() {
       _menuNavigationService.NavigateTo(MenuNavigationConstant.TodayMottoView);
    }
    
    public ICommand PlayAudioCommand { get; }
    public async Task PlayAudio() {
        await _audioPlayer.PlayAudioAsync(TodayWord.Word);
    }
}