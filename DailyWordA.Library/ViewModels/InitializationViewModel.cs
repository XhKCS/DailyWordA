using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Services;

namespace DailyWordA.Library.ViewModels;

public class InitializationViewModel : ViewModelBase {
    private readonly IWordStorage _wordStorage;
    private readonly IRootNavigationService _rootNavigationService;
    private readonly IWordFavoriteStorage _wordFavoriteStorage;

    public InitializationViewModel(IWordStorage wordStorage, 
        IRootNavigationService rootNavigationService,
        IWordFavoriteStorage wordFavoriteStorage) {
        _wordStorage = wordStorage;
        _rootNavigationService = rootNavigationService;
        _wordFavoriteStorage = wordFavoriteStorage;
        
        OnInitializedCommand = new AsyncRelayCommand(OnInitializedAsync);
    }
    
    public ICommand OnInitializedCommand { get; }

    private async Task OnInitializedAsync() {
        if (!_wordStorage.IsInitialized) {
            await _wordStorage.InitializeAsync();
        }
        
        if (!_wordFavoriteStorage.IsInitialized) {
            await _wordFavoriteStorage.InitializeAsync();
        }

        await Task.Delay(3000);

        _rootNavigationService.NavigateTo(RootNavigationConstant.MainView);
    }

}