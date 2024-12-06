using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;

namespace DailyWordA.Library.ViewModels;

public class WordleGameViewModel : ViewModelBase {
    private readonly IWordStorage _wordStorage;
    private readonly IContentNavigationService _contentNavigationService;
    private readonly IWordMistakeStorage _wordMistakeStorage;

    public WordleGameViewModel(IWordStorage wordStorage, 
        IContentNavigationService contentNavigationService,
        IWordMistakeStorage wordMistakeStorage) {
        _wordStorage = wordStorage;
        _contentNavigationService = contentNavigationService;
        _wordMistakeStorage = wordMistakeStorage;
        
        CommitCommand = new AsyncRelayCommand(CommitAsync);
        ShowDetailCommand = new RelayCommand(ShowDetail);
        UpdateCommand = new RelayCommand(Update);
        
        Update();
    }
    
    // 正确答案对应的单词
    private WordObject _correctWord;
    public WordObject CorrectWord {
        get => _correctWord;
        set => SetProperty(ref _correctWord, value);
    }

    public const int MaxAttemptsCount  = 6;

    public ObservableCollection<ObservableCollection<LetterStatus>> GridLetters { get; set; }

    //当前用户输入的是第几行（第几次尝试）
    private int _currentAttemptRow; 
    public int CurrentAttemptRow {
        get => _currentAttemptRow;
        set => SetProperty(ref _currentAttemptRow, value);
    }
    
    private string _hintText;
    public string HintText {
        get => _hintText;
        set => SetProperty(ref _hintText, value);
    }
    
    // 用户输入的
    private string _inputWord = string.Empty; //用户输入的单词文本
    public string InputWord {
        get => _inputWord;
        set => SetProperty(ref _inputWord, value);
    }
    
    private string _mask;
    public string Mask {
        get => _mask;
        private set => SetProperty(ref _mask, value);
    }
    
    private string _resultText;
    public string ResultText {
        get => _resultText;
        set => SetProperty(ref _resultText, value);
    }
    
    private bool _hasFinished; //已提交答案
    public bool HasFinished {
        get => _hasFinished;
        set => SetProperty(ref _hasFinished, value);
    }
    
    private bool _isLoading;
    public bool IsLoading {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    
    // 切换到下一题
    public ICommand UpdateCommand { get; }
    public void Update() {
        // 初始化时也需要调用
        Task.Run(async () => {
            IsLoading = true;
            HasFinished = false;
            InputWord = string.Empty;
            Mask = string.Empty;
            ResultText = string.Empty;
        
            CorrectWord = await _wordStorage.GetRandomWordAsync();
            while (CorrectWord.Word.Length > 5) {
                CorrectWord = await _wordStorage.GetRandomWordAsync();
            }
            for (int i = 0; i < CorrectWord.Word.Length; i++) {
                Mask += "L";
            }
            
            GridLetters = new ObservableCollection<ObservableCollection<LetterStatus>>();
            for (int i = 0; i < MaxAttemptsCount; i++) {
                var row = new ObservableCollection<LetterStatus>();
                for (int j = 0; j < CorrectWord.Word.Length; j++) {
                    row.Add(new LetterStatus{ Letter = " ", Background = "White"});
                }
                GridLetters.Add(row);
            }
            OnPropertyChanged(nameof(GridLetters));

            CurrentAttemptRow = 0;
            HintText = $"请输入第{CurrentAttemptRow+1}次猜的单词：";
            
            IsLoading = false;
        });
    }
    
    
    // 用户点击提交按钮
    public ICommand CommitCommand { get; }
    public async Task CommitAsync() {
        for (int j = 0; j < CorrectWord.Word.Length && j < InputWord.Length; j++) {
            var letter = InputWord.Substring(j, 1);
            var letterStatus = new LetterStatus { Letter = letter };
            if (!CorrectWord.Word.Contains(letter)) {
                letterStatus.Background = "Gray";
            }
            else {
                letterStatus.Background = CorrectWord.Word[j]==letter[0] ? "Green" : "Yellow";
            }
            GridLetters[CurrentAttemptRow][j] = letterStatus;
        }
        OnPropertyChanged(nameof(GridLetters));
        
        //提交后逻辑
        CurrentAttemptRow++;
        if (InputWord == CorrectWord.Word) {
            HasFinished = true;
            ResultText = $"恭喜您在{CurrentAttemptRow}次尝试后回答正确！";
        }
        else {
            if (CurrentAttemptRow >= MaxAttemptsCount) {
                HasFinished = true;
                await _wordMistakeStorage.SaveMistakeAsync(new WordMistake {
                    WordId = CorrectWord.Id,
                    IsInNote = true,
                    Timestamp = DateTime.Now
                });
                ResultText = $"正确答案是{CorrectWord.Word}  很遗憾，您没有回答正确哦~";
            }else {
                ResultText = "不对哦，再试试吧~";
                HintText = $"请输入第{CurrentAttemptRow+1}次猜的单词：";
                InputWord = string.Empty;
            }
        }
    }
    
    // 跳转至单词详情页
    public ICommand ShowDetailCommand { get; }
    public void ShowDetail() {
        _contentNavigationService.NavigateTo(
            ContentNavigationConstant.WordDetailView, CorrectWord);
    }
    
}

public class LetterStatus {
    public string Letter { get; set; }
    
    // "White" or "Gray" or "Green" or "Yellow"
    public string Background { get; set; }
}