using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;

namespace DailyWordA.Library.ViewModels;

// 单词测验-听音写词
public class WordDictationViewModel : ViewModelBase {
    private readonly IWordStorage _wordStorage;
    private readonly IContentNavigationService _contentNavigationService;
    private readonly IAudioPlayer _audioPlayer;
    private readonly IWordMistakeStorage _wordMistakeStorage;

    public WordDictationViewModel(IWordStorage wordStorage, 
        IContentNavigationService contentNavigationService, 
        IAudioPlayer audioPlayer,
        IWordMistakeStorage wordMistakeStorage) {
        _wordStorage = wordStorage;
        _contentNavigationService = contentNavigationService;
        _audioPlayer = audioPlayer;
        _wordMistakeStorage = wordMistakeStorage;
        
        UpdateCommand = new RelayCommand(Update);
        CommitCommand = new AsyncRelayCommand(CommitAsync);
        ShowDetailCommand = new RelayCommand(ShowDetail);
        PlayAudioCommand = new AsyncRelayCommand(PlayAudio);
        
        Update();
    }
    
    // 正确答案对应的单词
    private WordObject _correctWord;
    public WordObject CorrectWord {
        get => _correctWord;
        set => SetProperty(ref _correctWord, value);
    }

    private string _inputWord = string.Empty; //用户输入的单词文本
    public string InputWord {
        get => _inputWord;
        set => SetProperty(ref _inputWord, value);
    }
    
    private string _resultText = string.Empty;
    public string ResultText {
        get => _resultText;
        set => SetProperty(ref _resultText, value);
    }
    
    private bool _hasAnswered; //已提交答案
    public bool HasAnswered {
        get => _hasAnswered;
        set => SetProperty(ref _hasAnswered, value);
    }
    
    private bool _isShowCnMeaning;
    public bool IsShowCnMeaning {
        get => _isShowCnMeaning;
        set => SetProperty(ref _isShowCnMeaning, value);
    }
    
    public bool IsLoading {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    private bool _isLoading;
    
    // 切换到下一题
    public ICommand UpdateCommand { get; }
    public void Update() {
        // 初始化时也需要调用
        Task.Run(async () => {
            IsLoading = true;
            HasAnswered = false;
            IsShowCnMeaning = false;
            InputWord = string.Empty;
        
            CorrectWord = await _wordStorage.GetRandomWordAsync();
        
            IsLoading = false;
        });
    }
    
    // public ICommand ShowCnMeaningCommand { get; }
    // public void ShowCnMeaning() {
    //     IsShowCnMeaning = true;
    // }
    
    // 用户点击提交按钮
    public ICommand CommitCommand { get; }
    public async Task CommitAsync() {
        if (InputWord == CorrectWord.Word) {
            ResultText = "恭喜您回答正确！";
        }
        else {
            ResultText = "很遗憾，回答错误啦~";
            await _wordMistakeStorage.SaveMistakeAsync(new WordMistake {
                WordId = CorrectWord.Id,
                IsInNote = true,
                Timestamp = DateTime.Now
            });
        }
        HasAnswered = true;
    }

    // 跳转至单词详情页
    public ICommand ShowDetailCommand { get; }
    public void ShowDetail() {
        _contentNavigationService.NavigateTo(
            ContentNavigationConstant.WordDetailView, CorrectWord);
    }
    
    public ICommand PlayAudioCommand { get; }
    public async Task PlayAudio() {
        await _audioPlayer.PlayAudioAsync(CorrectWord.Word);
    }
}