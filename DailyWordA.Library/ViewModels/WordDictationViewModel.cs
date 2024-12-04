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

    public WordDictationViewModel(IWordStorage wordStorage, 
        IContentNavigationService contentNavigationService, 
        IAudioPlayer audioPlayer) {
        _wordStorage = wordStorage;
        _contentNavigationService = contentNavigationService;
        _audioPlayer = audioPlayer;
        
        UpdateCommand = new RelayCommand(Update);
        CommitCommand = new RelayCommand(Commit);
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
            InputWord = string.Empty;
        
            CorrectWord = await _wordStorage.GetRandomWordAsync();
        
            IsLoading = false;
        });
    }
    
    // 用户点击提交按钮
    public ICommand CommitCommand { get; }
    public void Commit() {
        ResultText = InputWord == CorrectWord.Word ? "恭喜您回答正确！" : "很遗憾，回答错误啦~";
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