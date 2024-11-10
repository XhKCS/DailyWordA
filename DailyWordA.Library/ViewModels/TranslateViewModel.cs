using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Services;

namespace DailyWordA.Library.ViewModels;

public class TranslateViewModel : ViewModelBase {
    private readonly ITranslateService _translateService;

    public TranslateViewModel(ITranslateService translateService) {
        _translateService = translateService;
        
        TranslateCommand = new AsyncRelayCommand(TranslateAsync);
    }
    
    private string _sourceText = string.Empty;
    public string SourceText {
        get => _sourceText;
        set => SetProperty(ref _sourceText, value);
    }
    
    private string _targetText = string.Empty;
    public string TargetText {
        get => _targetText;
        set => SetProperty(ref _targetText, value);
    }

    private TargetLanguageType _languageType = TargetLanguageType.ToChineseType;
    public TargetLanguageType LanguageType {
        get => _languageType;
        set => SetProperty(ref _languageType, value);
    }

    public ICommand TranslateCommand { get; }
    public async Task TranslateAsync() {
        string text = SourceText.Replace("\n", "").Replace(" ","").Replace("\t","").Replace("\r","");
        // Console.WriteLine("Translating---");
        TargetText = await _translateService.Translate(text, "auto", LanguageType.ToLanguage);
    }
}

public class TargetLanguageType
{
    public static readonly TargetLanguageType ToChineseType =
        new("中文", "zh");
    
    public static readonly TargetLanguageType ToEnglishType =
        new("英文", "en");
    
    public static readonly TargetLanguageType ToFrenchType =
        new("法语", "fra");
    
    public static readonly TargetLanguageType ToRussianType =
        new("俄语", "ru");
    
    public static readonly TargetLanguageType ToSpanishType =
        new("西班牙语", "spa");
    
    public static readonly TargetLanguageType ToArabicType =
        new("阿拉伯语", "ara");
    
    public static readonly TargetLanguageType ToJapaneseType =
        new("日语", "jp");

    public static List<TargetLanguageType> TargetLanguageTypes { get; } = 
        [ToChineseType, ToEnglishType, ToFrenchType, ToRussianType, 
            ToSpanishType, ToArabicType, ToJapaneseType];

    private TargetLanguageType(string name, string toLanguage) {
        Name = name;
        ToLanguage = toLanguage;
    }
    public string Name { get; }
    public string ToLanguage { get; }
}