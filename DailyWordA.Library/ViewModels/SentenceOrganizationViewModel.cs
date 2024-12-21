using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Helpers;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;

namespace DailyWordA.Library.ViewModels;

public class SentenceOrganizationViewModel : ViewModelBase {
    private readonly IDailyMottoService _dailyMottoService;

    public SentenceOrganizationViewModel(IDailyMottoService dailyMottoService) {
        _dailyMottoService = dailyMottoService;
        
        UpdateCommand = new RelayCommand(Update);
        CommitCommand = new RelayCommand(Commit);
        SelectWordCommand = new RelayCommand<Location>(SelectWord);
        
        Update();
    }
    
    // 单词列表，可点击选中组成句子
    public ObservableCollection<ObservableCollection<WordStatus>> WordStatusGroup { get; set; } = new();
    
    // 正确的句子
    private DailyMotto _correctSentence;
    public DailyMotto CorrectSentence {
        get => _correctSentence;
        set => SetProperty(ref _correctSentence, value);
    }
    
    // 用户当前选择的单词组
    private ObservableCollection<string> _currentWords = new();
    public ObservableCollection<string> CurrentWords {
        get => _currentWords;
        set => SetProperty(ref _currentWords, value);
    }
    
    // 用户当前拼接成的句子
    private string _currentSentence;
    public string CurrentSentence {
        get => _currentSentence;
        set => SetProperty(ref _currentSentence, value);
    }
    
    private bool _hasAnswered; //已提交答案
    public bool HasAnswered {
        get => _hasAnswered;
        set => SetProperty(ref _hasAnswered, value);
    }
    
    // 用户提交后的提示文本
    private string _resultText = string.Empty;
    public string ResultText {
        get => _resultText;
        set => SetProperty(ref _resultText, value);
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
            CurrentSentence = string.Empty;
            CurrentWords.Clear();
            WordStatusGroup.Clear();

            CorrectSentence = await _dailyMottoService.GetTodayMottoAsync();
            var words = CorrectSentence.Content.Split(" ");
            List<string> wordList = new List<string>(words);
            Random random = new Random();
            
            for (int i=0; wordList.Count > 0; i++)
            {
                var row = new ObservableCollection<WordStatus>();
                for (int j=0; wordList.Count > 0 && row.Count < 6; j++) {
                    int randomIndex = random.Next(wordList.Count);
                    row.Add(new WordStatus
                    {
                        Word = wordList[randomIndex], IsSelected = false, Background = "White",
                        Location = new Location{RowIndex = i, ColumnIndex = j}
                    });
                    wordList.RemoveAt(randomIndex);
                }
                WordStatusGroup.Add(row);
            }
            OnPropertyChanged(nameof(WordStatusGroup));
        
            IsLoading = false;
        });
    }
    
    public ICommand SelectWordCommand { get; }

    public void SelectWord(Location location)
    {
        var wordStatus = WordStatusGroup[location.RowIndex][location.ColumnIndex];
        if (wordStatus.IsSelected==true) {
            wordStatus.IsSelected = false;
            wordStatus.Background = "White";
            CurrentWords.Remove(wordStatus.Word);
        }
        else
        {
            wordStatus.IsSelected = true;
            wordStatus.Background = ColorConstant.CommonGreen;
            CurrentWords.Add(wordStatus.Word);
        }
        
        WordStatusGroup[location.RowIndex][location.ColumnIndex] = wordStatus;
        OnPropertyChanged(nameof(WordStatusGroup));
        CurrentSentence = string.Join(" ", CurrentWords);
    }
    
    // 用户点击提交按钮
    public ICommand CommitCommand { get; }
    public void Commit() {
        if (CurrentSentence == CorrectSentence.Content) {
            ResultText = "恭喜您回答正确！";
        }
        else {
            ResultText = "很遗憾，回答错误啦~";
        }
        HasAnswered = true;
    }
}

public class WordStatus {
    public string Word { get; set; }
    
    public Location Location { get; set; }
    
    public bool IsSelected { get; set; }
    
    // "White" or "Green"
    public string Background { get; set; }
}

public class Location
{
    public int RowIndex { get; set; }
    
    public int ColumnIndex { get; set; }
}