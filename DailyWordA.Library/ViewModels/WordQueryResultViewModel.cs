using System.Linq.Expressions;
using AvaloniaInfiniteScrolling;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;

namespace DailyWordA.Library.ViewModels;

public class WordQueryResultViewModel : ViewModelBase {
    private readonly IWordStorage _wordStorage;
    
    private readonly IContentNavigationService _contentNavigationService;

    private Expression<Func<WordObject, bool>> _where;

    public WordQueryResultViewModel(IWordStorage wordStorage, 
        IContentNavigationService contentNavigationService) {
        _wordStorage = wordStorage;
        _contentNavigationService = contentNavigationService;
        
        WordCollection = new AvaloniaInfiniteScrollCollection<WordObject> {
            OnCanLoadMore = () => _canLoadMore,
            OnLoadMore = async () => {
                Status = Loading;
                var wordList = await _wordStorage.GetWordsAsync(
                    _where, WordCollection.Count, PageSize);
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
        
        ShowWordDetailCommand = new RelayCommand<WordObject>(ShowWordDetail);
    }
    
    public override void SetParameter(object parameter) {
        if (parameter is not Expression<Func<WordObject, bool>> where) {
            return;
        }

        _where = where;
        _canLoadMore = true;
        WordCollection.Clear();
    }
    
    public IRelayCommand<WordObject> ShowWordDetailCommand { get; }

    public void ShowWordDetail(WordObject wordObject) {
        _contentNavigationService.NavigateTo(ContentNavigationConstant.WordDetailView, wordObject);
    }
    
    public AvaloniaInfiniteScrollCollection<WordObject> WordCollection { get;  }
    
    private bool _canLoadMore = true;
    
    private string _status;
    public string Status {
        get => _status;
        private set => SetProperty(ref _status, value); //注意前一个参数需要传ref，这样在函数内部就真的会修改_status的值（类似指针）
    }
    
    public const int PageSize = 50;
    
    public const string Loading = "正在载入";
    public const string NoResult = "没有满足条件的结果";
    public const string NoMoreResult = "没有更多结果";
}