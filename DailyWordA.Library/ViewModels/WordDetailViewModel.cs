using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;

namespace DailyWordA.Library.ViewModels;

public class WordDetailViewModel : ViewModelBase {
    private readonly IMenuNavigationService _menuNavigationService;
    private readonly IWordFavoriteStorage _wordFavoriteStorage;
    private readonly IAudioPlayer _audioPlayer;
    private readonly IWordMistakeStorage _wordMistakeStorage;

    public WordDetailViewModel(IMenuNavigationService menuNavigationService,
        IWordFavoriteStorage wordFavoriteStorage,
        IAudioPlayer audioPlayer,
        IWordMistakeStorage wordMistakeStorage) {
        _menuNavigationService = menuNavigationService;
        _wordFavoriteStorage = wordFavoriteStorage;
        _audioPlayer = audioPlayer;
        _wordMistakeStorage = wordMistakeStorage;
        
        OnLoadedCommand = new AsyncRelayCommand(OnLoadedAsync);
        FavoriteSwitchCommand = new AsyncRelayCommand(FavoriteSwitchClickedAsync);
        QueryCommand = new RelayCommand(Query);
        PlayAudioCommand = new AsyncRelayCommand(PlayAudio);
        MistakeSwitchCommand = new AsyncRelayCommand(MistakeSwitchClickedAsync);
    }
    
    private WordObject _currentWord;
    public WordObject CurrentWord {
        get => _currentWord;
        set => SetProperty(ref _currentWord, value);
    }
    
    public override void SetParameter(object parameter) {
        CurrentWord = parameter as WordObject;
    }

    public bool CanShowPhrase => _currentWord.Phrase is { Length: > 0 };
    
    public bool CanShowEtyma => _currentWord.Etyma is { Length: > 0 };

    
    public WordFavorite Favorite {
        get => _favorite;
        set => SetProperty(ref _favorite, value);
    }
    private WordFavorite _favorite;
    
    public WordMistake Mistake {
        get => _mistake;
        set => SetProperty(ref _mistake, value);
    }
    private WordMistake _mistake;
    
    public bool IsLoading {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    private bool _isLoading;
    
    // 页面初始化
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
        
        var mistake = await _wordMistakeStorage.GetMistakeAsync(CurrentWord.Id);
        if (mistake == null) {
            mistake = new WordMistake {
                WordId = CurrentWord.Id
            };
        }
        Mistake = mistake;
        
        IsLoading = false;
    }
    
    public ICommand FavoriteSwitchCommand { get; }
    public async Task FavoriteSwitchClickedAsync() {
        IsLoading = true;
        await _wordFavoriteStorage.SaveFavoriteAsync(Favorite);
        IsLoading = false;
    }
    
    public ICommand MistakeSwitchCommand { get; }
    public async Task MistakeSwitchClickedAsync() {
        IsLoading = true;
        await _wordMistakeStorage.SaveMistakeAsync(Mistake);
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