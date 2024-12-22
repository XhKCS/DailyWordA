using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Helpers;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using MvvmHelpers;

namespace DailyWordA.Library.ViewModels;

public class WordleGameViewModel : ViewModelBase {
    private readonly IWordStorage _wordStorage;
    private readonly IContentNavigationService _contentNavigationService;
    private readonly IWordMistakeStorage _wordMistakeStorage;
    private readonly IAlertService _alertService;

    public WordleGameViewModel(IWordStorage wordStorage, 
        IContentNavigationService contentNavigationService,
        IWordMistakeStorage wordMistakeStorage,
        IAlertService alertService) {
        _wordStorage = wordStorage;
        _contentNavigationService = contentNavigationService;
        _wordMistakeStorage = wordMistakeStorage;
        _alertService = alertService;
        
        CommitCommand = new AsyncRelayCommand(CommitAsync);
        ShowDetailCommand = new RelayCommand(ShowDetail);
        UpdateCommand = new RelayCommand(Update);
        ChangeSourceCommand = new AsyncRelayCommand(ChangeSource);
        
        Update();
    }
    
    // 页面中的方格阵列所绑定的数据
    public ObservableCollection<ObservableCollection<LetterStatus>> GridLetters { get; set; }
    
    // 正确答案对应的单词
    private WordObject _correctWord;
    public WordObject CorrectWord {
        get => _correctWord;
        set => SetProperty(ref _correctWord, value);
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
        if (IsFromMistake == true) {
            var mistakeList = await _wordMistakeStorage.GetMistakeListAsync();
            if (mistakeList.Count() == 0) {
                IsFromMistake = false;
                await _alertService.AlertAsync("当前错题本为空", "当前错题本还没有单词哦，已自动切换回默认来源~");
            }
            else {
                Update();
                await _alertService.AlertAsync("切换成功", "测验单词来源已切换为：仅来自错题本");
            }
            
        }
        else {
            Update();
            await _alertService.AlertAsync("切换成功", "测验单词来源已切换为：默认来源（从所有单词中随机抽取）");
        }
    }

    
    // 每轮最大尝试次数
    public const int MaxAttemptsCount  = 6;
    
    //当前用户输入的是第几行（第几次尝试）
    private int _currentAttemptRow; 
    public int CurrentAttemptRow {
        get => _currentAttemptRow;
        set => SetProperty(ref _currentAttemptRow, value);
    }
    
    // 提示用户输入
    private string _hintText;
    public string HintText {
        get => _hintText;
        set => SetProperty(ref _hintText, value);
    }
    
    // 用户输入的文本
    private string _inputWord = string.Empty; //用户输入的单词文本
    public string InputWord {
        get => _inputWord;
        set => SetProperty(ref _inputWord, value.ToLower());
    }
    
    // MaskedTextBox的Mask
    private string _mask;
    public string Mask {
        get => _mask;
        private set => SetProperty(ref _mask, value);
    }
    
    // 用户提交后的提示文本
    private string _resultText;
    public string ResultText {
        get => _resultText;
        set => SetProperty(ref _resultText, value);
    }
    
    // 游戏是否结束
    private bool _hasFinished; 
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
            else {
                CorrectWord = await _wordStorage.GetRandomWordAsync();
                while (CorrectWord.Word.Length > 5) {
                    CorrectWord = await _wordStorage.GetRandomWordAsync();
                }
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
        // 更新GridLetters
        for (int j = 0; j < CorrectWord.Word.Length && j < InputWord.Length; j++) {
            var letter = InputWord.Substring(j, 1);
            var letterStatus = new LetterStatus { Letter = letter };
            if (!CorrectWord.Word.Contains(letter)) {
                letterStatus.Background = ColorConstant.CommonGrey;
            }
            else {
                letterStatus.Background = CorrectWord.Word[j]==letter[0] ? ColorConstant.CommonGreen : ColorConstant.CommonYellow;
            }
            GridLetters[CurrentAttemptRow][j] = letterStatus;
        }
        OnPropertyChanged(nameof(GridLetters));
        
        //提交后逻辑
        CurrentAttemptRow++;
        if (InputWord == CorrectWord.Word) {
            HasFinished = true; // 游戏结束且用户猜对了
            ResultText = $"恭喜您在{CurrentAttemptRow}次尝试后回答正确！";
            if (IsFromMistake) {
                await _wordMistakeStorage.SaveMistakeAsync(new WordMistake {
                    WordId = CorrectWord.Id,
                    IsInNote = false,
                    Timestamp = DateTime.Now
                });
            }
        }
        else {
            if (CurrentAttemptRow >= MaxAttemptsCount) {
                HasFinished = true; //游戏结束且用户没有猜对，需要将该单词加入错题本
                await _wordMistakeStorage.SaveMistakeAsync(new WordMistake {
                    WordId = CorrectWord.Id,
                    IsInNote = true,
                    Timestamp = DateTime.Now
                });
                ResultText = $"正确答案是{CorrectWord.Word}  很遗憾，您没有回答正确哦~";
            }else {
                // 游戏还没结束，用户还有尝试机会
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