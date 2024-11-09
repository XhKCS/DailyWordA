using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using MvvmHelpers;

namespace DailyWordA.Library.ViewModels;

public class WordFavoriteViewModel : ViewModelBase {
    private readonly IWordFavoriteStorage _favoriteStorage;
    private readonly IWordStorage _wordStorage;
    private readonly IContentNavigationService _contentNavigationService;

    public WordFavoriteViewModel(IWordFavoriteStorage favoriteStorage, 
        IWordStorage wordStorage, 
        IContentNavigationService contentNavigationService) {
        _favoriteStorage = favoriteStorage;
        _wordStorage = wordStorage;
        _contentNavigationService = contentNavigationService;

        _favoriteStorage.Updated += FavoriteStorageOnUpdated;
        
        OnInitializedCommand = new AsyncRelayCommand(OnInitializedAsync);
        ShowWordDetailCommand = new RelayCommand<WordObject>(ShowWordDetail);
    }
    
    public ObservableRangeCollection<WordFavoriteCombination> WordFavoriteCollection { get; } = new();
    
    public bool IsLoading {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    private bool _isLoading;
    
    public ICommand OnInitializedCommand { get; }
    public async Task OnInitializedAsync() {
        IsLoading = true;

        WordFavoriteCollection.Clear();
        var favoriteList = await _favoriteStorage.GetFavoriteListAsync();

        WordFavoriteCollection.AddRange((await Task.WhenAll(
            favoriteList.Select(p => Task.Run(async () => new WordFavoriteCombination {
                WordObject = await _wordStorage.GetWordAsync(p.WordId), 
                Favorite = p
            })))).ToList());

        IsLoading = false;
    }
    
    public IRelayCommand<WordObject> ShowWordDetailCommand { get; }

    public void ShowWordDetail(WordObject wordObject) {
        _contentNavigationService.NavigateTo(ContentNavigationConstant.WordDetailView, wordObject);
    }
    
    private async void FavoriteStorageOnUpdated(object sender,
        FavoriteStorageUpdatedEventArgs e) {
        var favorite = e.UpdatedFavorite;

        if (!favorite.IsFavorite) {
            WordFavoriteCollection.Remove(
                WordFavoriteCollection.FirstOrDefault(p =>
                    p.Favorite.WordId == favorite.WordId));
            return;
        }

        var wordFavoriteCombination = new WordFavoriteCombination {
            WordObject = await _wordStorage.GetWordAsync(favorite.WordId), 
            Favorite = favorite
        };

        var index = WordFavoriteCollection.IndexOf(
            WordFavoriteCollection.FirstOrDefault(p =>
                p.Favorite.Timestamp < favorite.Timestamp));
        if (index < 0) {
            index = WordFavoriteCollection.Count;
        }

        WordFavoriteCollection.Insert(index, wordFavoriteCombination);
    }
}

public class WordFavoriteCombination {
    public WordObject WordObject { get; set; }
    
    public WordFavorite Favorite { get; set; }
}

