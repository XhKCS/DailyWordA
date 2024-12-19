using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;

namespace DailyWordA.Library.ViewModels;

// 单词测验-例句填空
public class WordFillingViewModel : ViewModelBase {
    private readonly IWordStorage _wordStorage;
    private readonly IContentNavigationService _contentNavigationService;
    private readonly IWordMistakeStorage _wordMistakeStorage;
    
    public WordFillingViewModel(IWordStorage wordStorage, 
        IContentNavigationService contentNavigationService,
        IWordMistakeStorage wordMistakeStorage) {
        _wordStorage = wordStorage;
        _contentNavigationService = contentNavigationService;
        _wordMistakeStorage = wordMistakeStorage;
            
        UpdateCommand = new RelayCommand(Update);
        CommitCommand = new AsyncRelayCommand(CommitAsync);
        ShowDetailCommand = new RelayCommand(ShowDetail);
            
        Update();
    }
    
    // 正确答案对应的单词
    private WordObject _correctWord;
    public WordObject CorrectWord {
        get => _correctWord;
        set => SetProperty(ref _correctWord, value);
    }
    
    // 填空的正确答案，与CorrectWord.Word不一定相同哦
    private string _correctFillingText;
    public string CorrectFillingText {
        get => _correctFillingText;
        set => SetProperty(ref _correctFillingText, value);
    }
    
    // 展示给用户的填空题目
    private string _showBlankText;
    public string ShowBlankText {
        get => _showBlankText;
        set => SetProperty(ref _showBlankText, value);
    }
    
    //用户输入的单词文本
    private string _inputWord = string.Empty; 
    public string InputWord {
        get => _inputWord;
        set => SetProperty(ref _inputWord, value.Replace(" ", "").ToLower());
    }
    
    private bool _hasAnswered; //已提交答案
    public bool HasAnswered {
        get => _hasAnswered;
        set => SetProperty(ref _hasAnswered, value);
    }
    
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
            ShowBlankText = string.Empty;
            InputWord = string.Empty;
        
            CorrectWord = await _wordStorage.GetRandomWordAsync();
            
            string[] splitWords = CorrectWord.Sentence.Split(new char[] { ' ', ',', '.', '!','?', '\'', ':','/','~' }, StringSplitOptions.RemoveEmptyEntries);
            CorrectFillingText = splitWords[0];
            string tempWord = CorrectWord.Word;
            bool hasFound = false;
            do {
                foreach (var word in splitWords) {
                    if (word.Contains(tempWord)) {
                        CorrectFillingText = word;
                        hasFound = true;
                    }
                }
                tempWord = tempWord.Substring(0, tempWord.Length - 1);
            } while (hasFound == false && tempWord.Length > 0);
            
            var blank = ""+CorrectFillingText[0];
            for (int i = 1; i < CorrectFillingText.Length; i++) {
                blank += "_";
            }
            ShowBlankText = CorrectWord.Sentence.Replace(CorrectFillingText, blank);
            // Console.WriteLine(ShowBlankText);
            // Console.WriteLine(CorrectFillingText);
        
            IsLoading = false;
        });
    }
    
    // 用户点击提交按钮
    public ICommand CommitCommand { get; }
    public async Task CommitAsync() {
        if (InputWord == CorrectFillingText) {
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
}