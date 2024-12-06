using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Services;

namespace DailyWordA.Library.ViewModels;

public class MainWindowViewModel : ViewModelBase {
    private readonly IWordStorage _wordStorage;
    private readonly IRootNavigationService _rootNavigationService;
    private readonly IWordFavoriteStorage _wordFavoriteStorage;
    private readonly IWordMistakeStorage _wordMistakeStorage;

    public MainWindowViewModel(IWordStorage wordStorage, 
        IRootNavigationService rootNavigationService,
        IWordFavoriteStorage wordFavoriteStorage,
        IWordMistakeStorage wordMistakeStorage) {
        _wordStorage = wordStorage;
        _rootNavigationService = rootNavigationService;
        _wordFavoriteStorage = wordFavoriteStorage;
        _wordMistakeStorage = wordMistakeStorage;
        
        OnInitializedCommand = new RelayCommand(OnInitialized);
    }

    private ViewModelBase _content;
    
    // 内部提供一个ViewModel
    public ViewModelBase Content {
        get => _content;
        set => SetProperty(ref _content, value);
    }
    
    public ICommand OnInitializedCommand { get; }

    public void OnInitialized() {
        if (!_wordStorage.IsInitialized || !_wordFavoriteStorage.IsInitialized
            || !_wordMistakeStorage.IsInitialized) {
            _rootNavigationService.NavigateTo(RootNavigationConstant.InitializationView);
        }
        else {
            _rootNavigationService.NavigateTo(RootNavigationConstant.MainView);
        }
    }
}