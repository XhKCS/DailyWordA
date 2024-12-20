using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using MvvmHelpers;

namespace DailyWordA.Library.ViewModels;

// 单词测验-听音写词
public class WordDictationViewModel : ViewModelBase {
    private readonly IWordStorage _wordStorage;
    private readonly IContentNavigationService _contentNavigationService;
    private readonly IAudioPlayer _audioPlayer;
    private readonly IWordMistakeStorage _wordMistakeStorage;
    private readonly IAlertService _alertService;

    public WordDictationViewModel(IWordStorage wordStorage, 
        IContentNavigationService contentNavigationService, 
        IAudioPlayer audioPlayer,
        IWordMistakeStorage wordMistakeStorage,
        IAlertService alertService) {
        _wordStorage = wordStorage;
        _contentNavigationService = contentNavigationService;
        _audioPlayer = audioPlayer;
        _wordMistakeStorage = wordMistakeStorage;
        _alertService = alertService;
        
        UpdateCommand = new RelayCommand(Update);
        CommitCommand = new AsyncRelayCommand(CommitAsync);
        ShowDetailCommand = new RelayCommand(ShowDetail);
        PlayAudioCommand = new AsyncRelayCommand(PlayAudio);
        ChangeSourceCommand = new AsyncRelayCommand(ChangeSource);
        
        Update();
    }
    
    // 单词是否来源于错题本
    private bool _isFromMistakes = false;
    public bool IsFromMistake
    {
        get => _isFromMistakes;
        set => SetProperty(ref _isFromMistakes, value);
    }
    
    private ObservableRangeCollection<WordObject> WordsFromMistakes { get; } = new();
    
    // 点击按钮切换单词来源
    public ICommand ChangeSourceCommand { get; }
    public async Task ChangeSource() {
        // 当触发该函数时，IsFromMistake的值已经改变了
        Update();
        if (IsFromMistake == true) {
            var mistakeList = await _wordMistakeStorage.GetMistakeListAsync();
            if (mistakeList.Count() == 0) {
                IsFromMistake = false;
                await _alertService.AlertAsync("当前错题本为空", "当前错题本还没有单词哦，已自动切换回默认来源~");
            }
            else {
                await _alertService.AlertAsync("切换成功", "测验单词来源已切换为：仅来自错题本");
            }
            
        }
        else {
            await _alertService.AlertAsync("切换成功", "测验单词来源已切换为：默认来源（从所有单词中随机抽取）");
        }
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
        set => SetProperty(ref _inputWord, value.Replace(" ", "").ToLower());
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
        
            if (_isFromMistakes) {
                WordsFromMistakes.Clear();
                var mistakeList = await _wordMistakeStorage.GetMistakeListAsync();

                WordsFromMistakes.AddRange((await Task.WhenAll(
                    mistakeList.Select(p => Task.Run(async () => await _wordStorage.GetWordAsync(p.WordId)))
                )).ToList());
                
                if (WordsFromMistakes.Count == 0) {
                    CorrectWord = await _wordStorage.GetRandomWordAsync();
                    // await _alertService.AlertAsync("当前错题本为空", "当前错题本还没有单词哦，已自动退出错题练习模式~");
                    IsFromMistake = false;
                }
                else {
                    CorrectWord = WordsFromMistakes[new Random().Next(0, WordsFromMistakes.Count)];
                }
            }
            else
            {
                CorrectWord = await _wordStorage.GetRandomWordAsync();
            }
        
            IsLoading = false;
        });
    }
    
    // 用户点击提交按钮
    public ICommand CommitCommand { get; }
    public async Task CommitAsync() {
        if (InputWord == CorrectWord.Word) {
            ResultText = "恭喜您回答正确！";
            if (IsFromMistake) {
                await _wordMistakeStorage.SaveMistakeAsync(new WordMistake {
                    WordId = CorrectWord.Id,
                    IsInNote = false,
                    Timestamp = DateTime.Now
                });
            }
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