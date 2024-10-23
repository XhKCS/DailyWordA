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

    public ICommand TranslateCommand { get; }
    public async Task TranslateAsync() {
        Console.WriteLine("Translating---");
        TargetText = await _translateService.Translate(SourceText);
    }
}