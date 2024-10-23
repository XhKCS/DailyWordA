using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Services;

namespace DailyWordA.Library.ViewModels;

public class MainWindowViewModel : ViewModelBase {
    private readonly IWordStorage _wordStorage;
    private readonly IRootNavigationService _rootNavigationService;

    public MainWindowViewModel(IWordStorage wordStorage, 
        IRootNavigationService rootNavigationService) {
        _wordStorage = wordStorage;
        _rootNavigationService = rootNavigationService;
        
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
        _rootNavigationService.NavigateTo(RootNavigationConstant.MainView);
    }
}