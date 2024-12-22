using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using MvvmHelpers;

namespace DailyWordA.Library.ViewModels;

public class MottoFavoriteViewModel : ViewModelBase {
    private readonly IMottoFavoriteStorage _mottoFavoriteStorage;
    private readonly IContentNavigationService _contentNavigationService;

    public MottoFavoriteViewModel(IMottoFavoriteStorage mottoFavoriteStorage,
        IContentNavigationService contentNavigationService) {
        _mottoFavoriteStorage = mottoFavoriteStorage;
        _contentNavigationService = contentNavigationService;

        _mottoFavoriteStorage.Updated += FavoriteStorageOnUpdated;
        
        OnInitializedCommand = new AsyncRelayCommand(OnInitializedAsync);
        ShowMottoDetailCommand = new RelayCommand<DailyMotto>(ShowMottoDetail);
    }
    
    public ObservableRangeCollection<DailyMotto> FavoriteMottoCollection { get; } = new();
    
    public bool IsLoading {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    private bool _isLoading;
    
    public ICommand OnInitializedCommand { get; }
    public async Task OnInitializedAsync() {
        IsLoading = true;

        FavoriteMottoCollection.Clear();
        var favoriteMottoList = await _mottoFavoriteStorage.GetFavoriteMottoListAsync();

        FavoriteMottoCollection.AddRange(favoriteMottoList);

        IsLoading = false;
    }
    
    public IRelayCommand<DailyMotto> ShowMottoDetailCommand { get; }

    public void ShowMottoDetail(DailyMotto dailyMotto) {
        _contentNavigationService.NavigateTo(ContentNavigationConstant.MottoDetailView, dailyMotto);
    }
    
    private async void FavoriteStorageOnUpdated(object sender,
        FavoriteMottoStorageUpdatedEventArgs e) {
        var updatedMotto = e.UpdatedFavoriteMotto;
        
        FavoriteMottoCollection.Remove(
           FavoriteMottoCollection.FirstOrDefault(p =>
                p.Id == updatedMotto.Id));

        if (e.IsFavorite == false) {
            return;
        }

        var index = FavoriteMottoCollection.IndexOf(
            FavoriteMottoCollection.FirstOrDefault(p =>
                p.FTimestamp < updatedMotto.FTimestamp));
        if (index < 0) {
            index = FavoriteMottoCollection.Count;
        }

        FavoriteMottoCollection.Insert(index, updatedMotto);
    }

}