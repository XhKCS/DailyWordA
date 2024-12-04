using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;

namespace DailyWordA.Library.ViewModels;

public class WordDetailViewModel : ViewModelBase {
    private readonly IMenuNavigationService _menuNavigationService;
    private readonly IWordFavoriteStorage _wordFavoriteStorage;
    private readonly IAudioPlayer _audioPlayer;

    public WordDetailViewModel(IMenuNavigationService menuNavigationService,
        IWordFavoriteStorage wordFavoriteStorage,
        IAudioPlayer audioPlayer) {
        _menuNavigationService = menuNavigationService;
        _wordFavoriteStorage = wordFavoriteStorage;
        _audioPlayer = audioPlayer;
        
        OnLoadedCommand = new AsyncRelayCommand(OnLoadedAsync);
        FavoriteSwitchCommand = new AsyncRelayCommand(FavoriteSwitchClickedAsync);
        QueryCommand = new RelayCommand(Query);
        PlayAudioCommand = new AsyncRelayCommand(PlayAudio);
    }
    
    private WordObject _currentWord;
    public WordObject CurrentWord {
        get => _currentWord;
        set => SetProperty(ref _currentWord, value);
    }

    public bool CanShowPhrase => _currentWord.Phrase is { Length: > 0 };
    
    public bool CanShowEtyma => _currentWord.Etyma is { Length: > 0 };

    public override void SetParameter(object parameter) {
        CurrentWord = parameter as WordObject;
    }
    
    public WordFavorite Favorite {
        get => _favorite;
        set => SetProperty(ref _favorite, value);
    }
    private WordFavorite _favorite;
    
    public bool IsLoading {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    private bool _isLoading;
    
    public ICommand OnLoadedCommand { get; }
    public async Task OnLoadedAsync() {
        IsLoading = true;
        var favorite = await _wordFavoriteStorage.GetFavoriteAsync(CurrentWord.Id);
        if (favorite == null) {
            favorite = new WordFavorite {
                WordId = CurrentWord.Id
            };
        }
        Favorite = favorite;
        IsLoading = false;
    }
    
    public ICommand FavoriteSwitchCommand { get; }

    public async Task FavoriteSwitchClickedAsync() {
        IsLoading = true;
        await _wordFavoriteStorage.SaveFavoriteAsync(Favorite);
        IsLoading = false;
    }
    
    public ICommand QueryCommand { get; }
    public void Query() {
        _menuNavigationService.NavigateTo(MenuNavigationConstant.WordQueryView, 
            new WordQuery {
                Word = CurrentWord.Word, CnMeaning = CurrentWord.CnMeaning
            });
    }
    
    public ICommand PlayAudioCommand { get; }
    public async Task PlayAudio() {
        await _audioPlayer.PlayAudioAsync(CurrentWord.Word);
    }

}