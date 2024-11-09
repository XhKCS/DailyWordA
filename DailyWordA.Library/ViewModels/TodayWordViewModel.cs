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
    
    private readonly IMenuNavigationService _menuNavigationService;
    
    public  TodayWordViewModel(IWordStorage wordStorage, 
        ITodayImageService todayImageService,
        IContentNavigationService contentNavigationService,
        IMenuNavigationService menuNavigationService) {
        _wordStorage = wordStorage;
        _todayImageService = todayImageService;
        _contentNavigationService = contentNavigationService;
        _menuNavigationService = menuNavigationService;
        if (_wordStorage.IsInitialized == false) {
            // 不要使用RunSynchronously，否则似乎会一直卡住
            Console.WriteLine("begin initializing wordStorage");
            _wordStorage.InitializeAsync();
        }
        // _wordStorage.InitializeAsync();
        
        // OnInitializedCommand = new AsyncRelayCommand(OnInitializedAsync);
        OnInitialized();
        UpdateWordCommand = new AsyncRelayCommand(UpdateWordAsync);
        ShowDetailCommand = new RelayCommand(ShowDetail);
        NavigateToTodayMottoViewCommand = new RelayCommand(NavigateToTodayMottoView);

        // WordCollection = new AvaloniaInfiniteScrollCollection<WordObject> {
        //     OnCanLoadMore = () => _canLoadMore,
        //     OnLoadMore = async () => {
        //         Status = Loading;
        //         var wordList = await _wordStorage.GetWordsAsync(
        //             Expression.Lambda<Func<WordObject, bool>>(
        //                 Expression.Constant(true),
        //                 Expression.Parameter(typeof(WordObject), "p")),
        //             WordCollection.Count, PageSize);
        //         Status = string.Empty;
        //
        //         if (wordList.Count < PageSize) {
        //             _canLoadMore = false;
        //             Status = NoMoreResult;
        //         }
        //         if (WordCollection.Count == 0 && wordList.Count == 0) {
        //             Status = NoResult;
        //         }
        //
        //         return wordList;
        //     }
        // };
    }
    
    public AvaloniaInfiniteScrollCollection<WordObject> WordCollection { get;  }
    
    private bool _canLoadMore = true;
    
    private string _status;
    public string Status {
        get => _status;
        private set => SetProperty(ref _status, value); //注意前一个参数需要传ref，这样在函数内部就真的会修改_status的值（类似指针）
    }
    
    public const int PageSize = 20;
    
    public const string Loading = "正在载入";
    public const string NoResult = "没有满足条件的结果";
    public const string NoMoreResult = "没有更多结果";

    // 今日推荐单词
    private WordObject _todayWord;
    public WordObject TodayWord {
        get => _todayWord;
        set => SetProperty(ref _todayWord, value);
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
    
    public ICommand OnInitializedCommand { get; }
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
            TodayWord = await _wordStorage.GetRandomWordAsync();
            await Task.Delay(200);
            IsLoading = false;
        });
    }

    public ICommand UpdateWordCommand { get; }
    private async Task UpdateWordAsync() {
        IsLoading = true;
        TodayWord = await _wordStorage.GetRandomWordAsync();
        await Task.Delay(300);
        IsLoading = false;
    }
    
    // 跳转至单词详情页
    public ICommand ShowDetailCommand { get; }
    public void ShowDetail() {
        // 跳转至详情页面，注意要传参：当前的TodayPoetry
        _contentNavigationService.NavigateTo(
            ContentNavigationConstant.WordDetailView, TodayWord);
    }
    
    public ICommand NavigateToTodayMottoViewCommand { get; }
    private void NavigateToTodayMottoView() {
       _menuNavigationService.NavigateTo(MenuNavigationConstant.TodayMottoView);
    }
}