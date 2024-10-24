using System.Linq.Expressions;
using System.Windows.Input;
using AvaloniaInfiniteScrolling;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;

namespace DailyWordA.Library.ViewModels;

public class WordResultViewModel : ViewModelBase {
    private readonly IWordStorage _wordStorage;
    
    private readonly IRootNavigationService _rootNavigationService;
    
    public  WordResultViewModel(IWordStorage wordStorage, IRootNavigationService rootNavigationService) {
        _wordStorage = wordStorage;
        _rootNavigationService = rootNavigationService;
        // if (_wordStorage.IsInitialized == false) {
        //     // 不要使用RunSynchronously，否则似乎会一直卡住
        //     Console.WriteLine("begin initializing wordStorage");
        //     _wordStorage.InitializeAsync();
        // }
        
        // Console.WriteLine("begin initializing wordStorage");
        _wordStorage.InitializeAsync();
        // Console.WriteLine("WordStorage initialization exited.");
        
        OnInitializedCommand = new AsyncRelayCommand(OnInitializedAsync);
        NavigateToTodayMottoViewCommand = new RelayCommand(NavigateToTodayMottoView);

        WordCollection = new AvaloniaInfiniteScrollCollection<WordObject> {
            OnCanLoadMore = () => _canLoadMore,
            OnLoadMore = async () => {
                Status = Loading;
                var wordList = await _wordStorage.GetWordsAsync(
                    Expression.Lambda<Func<WordObject, bool>>(
                        Expression.Constant(true),
                        Expression.Parameter(typeof(WordObject), "p")),
                    WordCollection.Count, PageSize);
                Status = string.Empty;

                if (wordList.Count < PageSize) {
                    _canLoadMore = false;
                    Status = NoMoreResult;
                }
                if (WordCollection.Count == 0 && wordList.Count == 0) {
                    Status = NoResult;
                }

                return wordList;
            }
        };
    }
    
    public AvaloniaInfiniteScrollCollection<WordObject> WordCollection { get;  }
    
    private bool _canLoadMore = true;
    
    private string _status;
    public string Status {
        get => _status;
        //必须用SetProperty函数才能触发PropertyChanged事件，直接用等号给属性赋值不能触发事件
        private set => SetProperty(ref _status, value); //注意前一个参数需要传ref，这样在函数内部就真的会修改_status的值（类似指针）
        //MVVM的本质：View会自动订阅ViewModel的PropertyChanged事件，从而第一时间知道ViewModel中属性的变化，取出ViewModel里属性的新值并刷新页面
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
    
    private bool _isLoading;
    public bool IsLoading {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }
    
    public ICommand OnInitializedCommand { get; }

    public async Task OnInitializedAsync() {
        IsLoading = true;
        TodayWord = await _wordStorage.GetRandomWordAsync();
        IsLoading = false;
    }
    
    public ICommand NavigateToTodayMottoViewCommand { get; }

    private void NavigateToTodayMottoView() {
       _rootNavigationService.NavigateTo(nameof(TodayMottoViewModel));
    }
}