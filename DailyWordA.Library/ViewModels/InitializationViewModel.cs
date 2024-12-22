using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Services;

namespace DailyWordA.Library.ViewModels;

public class InitializationViewModel : ViewModelBase {
    private readonly IWordStorage _wordStorage;
    private readonly IRootNavigationService _rootNavigationService;
    private readonly IWordFavoriteStorage _wordFavoriteStorage;
    private readonly IWordMistakeStorage _wordMistakeStorage;
    private readonly IMottoFavoriteStorage _mottoFavoriteStorage;

    public InitializationViewModel(IWordStorage wordStorage, 
        IRootNavigationService rootNavigationService,
        IWordFavoriteStorage wordFavoriteStorage,
        IWordMistakeStorage wordMistakeStorage,
        IMottoFavoriteStorage mottoFavoriteStorage) {
        _wordStorage = wordStorage;
        _rootNavigationService = rootNavigationService;
        _wordFavoriteStorage = wordFavoriteStorage;
        _wordMistakeStorage = wordMistakeStorage;
        _mottoFavoriteStorage = mottoFavoriteStorage;
        
        OnInitializedCommand = new AsyncRelayCommand(OnInitializedAsync);
    }
    
    public ICommand OnInitializedCommand { get; }

    public async Task OnInitializedAsync() {
        if (!_wordStorage.IsInitialized) {
            await _wordStorage.InitializeAsync();
        }
        
        if (!_wordFavoriteStorage.IsInitialized) {
            await _wordFavoriteStorage.InitializeAsync();
        }

        if (!_wordMistakeStorage.IsInitialized) {
            await _wordMistakeStorage.InitializeAsync();
        }

        if (!_mottoFavoriteStorage.IsInitialized) {
            await _mottoFavoriteStorage.InitializeAsync();
        }

        await Task.Delay(2000);

        _rootNavigationService.NavigateTo(RootNavigationConstant.MainView);
    }

}