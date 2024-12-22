using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;

namespace DailyWordA.Library.ViewModels;

public class MottoDetailViewModel : ViewModelBase {
    private readonly IMottoFavoriteStorage _mottoFavoriteStorage;

    public MottoDetailViewModel(IMottoFavoriteStorage mottoFavoriteStorage) {
        _mottoFavoriteStorage = mottoFavoriteStorage;
        
        OnLoadedCommand = new AsyncRelayCommand(OnLoadedAsync);
        FavoriteSwitchCommand = new AsyncRelayCommand(FavoriteSwitchClickedAsync);
    }
    
    private DailyMotto _currentMotto;
    public DailyMotto CurrentMotto {
        get => _currentMotto;
        private set => SetProperty(ref _currentMotto, value);
    }

    private bool _isFavorite = false;
    public bool IsFavorite {
        get => _isFavorite;
        set => SetProperty(ref _isFavorite, value);
    }
    
    public override void SetParameter(object parameter) {
        CurrentMotto = parameter as DailyMotto;
    }
    
    public bool CanShowSource => _currentMotto.Source!=null && _currentMotto.Source.Length > 0;
        
    public bool CanShowAuthor => _currentMotto.Author!=null && _currentMotto.Author.Length > 0;
    
    public bool IsLoading {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    private bool _isLoading;

    // 页面初始化
    public ICommand OnLoadedCommand { get; }
    public async Task OnLoadedAsync() {
        IsLoading = true;
        IsFavorite = false;
        var favoriteMotto = await _mottoFavoriteStorage.GetFavoriteMottoAsync(CurrentMotto.Id);
        if (favoriteMotto != null) {
            IsFavorite = true;
        }
        
        IsLoading = false;
    }
    
    public ICommand FavoriteSwitchCommand { get; }
    public async Task FavoriteSwitchClickedAsync() {
        IsLoading = true;
        if (IsFavorite == true) {
            await _mottoFavoriteStorage.InsertFavoriteMottoAsync(CurrentMotto);
        }
        else {
            await _mottoFavoriteStorage.DeleteFavoriteMottoAsync(CurrentMotto);
        }
        
        IsLoading = false;
    }
}