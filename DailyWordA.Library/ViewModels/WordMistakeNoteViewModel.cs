using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using MvvmHelpers;

namespace DailyWordA.Library.ViewModels;

public class WordMistakeNoteViewModel : ViewModelBase {
    private readonly IWordMistakeStorage _wordMistakeStorage;
    private readonly IWordStorage _wordStorage;
    private readonly IContentNavigationService _contentNavigationService;

    public WordMistakeNoteViewModel(IWordMistakeStorage wordMistakeStorage, 
        IWordStorage wordStorage, 
        IContentNavigationService contentNavigationService) {
        _wordMistakeStorage = wordMistakeStorage;
        _wordStorage = wordStorage;
        _contentNavigationService = contentNavigationService;

        _wordMistakeStorage.Updated += MistakeStorageOnUpdated;
        
        OnInitializedCommand = new AsyncRelayCommand(OnInitializedAsync);
        ShowWordDetailCommand = new RelayCommand<WordObject>(ShowWordDetail);
        // OnWordMistakeCheckedCommand = new RelayCommand<WordMistake>(OnWordMistakeChecked);
    }
    
    public ObservableRangeCollection<WordMistakeNote> WordMistakeNoteCollection { get; } = new();
    
    // public ObservableRangeCollection<WordMistake> SelectedWordMistakes { get; } = new();
    
    public bool IsLoading {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    private bool _isLoading;
    
    public ICommand OnInitializedCommand { get; }
    public async Task OnInitializedAsync() {
        IsLoading = true;

        WordMistakeNoteCollection.Clear();
        var mistakeList = await _wordMistakeStorage.GetMistakeListAsync();

        WordMistakeNoteCollection.AddRange((await Task.WhenAll(
            mistakeList.Select(p => Task.Run(async () => new WordMistakeNote {
                WordObject = await _wordStorage.GetWordAsync(p.WordId),
                WordMistake = p
            })))).ToList());

        IsLoading = false;
    }
    
    public IRelayCommand<WordObject> ShowWordDetailCommand { get; }

    public void ShowWordDetail(WordObject wordObject) {
        _contentNavigationService.NavigateTo(ContentNavigationConstant.WordDetailView, wordObject);
    }
    
    private async void MistakeStorageOnUpdated(object sender,
        MistakeStorageUpdatedEventArgs e) {
        var updatedMistake = e.UpdatedMistake;
        WordMistakeNoteCollection.Remove(
            WordMistakeNoteCollection.FirstOrDefault(p =>
                p.WordMistake.WordId == updatedMistake.WordId));

        if (!updatedMistake.IsInNote) {
            return;
        }

        var wordMistakeNote = new WordMistakeNote {
            WordObject = await _wordStorage.GetWordAsync(updatedMistake.WordId), 
            WordMistake = updatedMistake
        };

        var index = WordMistakeNoteCollection.IndexOf(
            WordMistakeNoteCollection.FirstOrDefault(p =>
                p.WordMistake.Timestamp < updatedMistake.Timestamp));
        if (index < 0) {
            index = WordMistakeNoteCollection.Count;
        }

        WordMistakeNoteCollection.Insert(index, wordMistakeNote);
    }
    
    // public ICommand OnWordMistakeCheckedCommand { get; }
    // public void OnWordMistakeChecked(WordMistake wordMistake) {
    //     SelectedWordMistakes.Add(wordMistake);
    //     Console.WriteLine("SelectedWordMistakes.Count: "+SelectedWordMistakes.Count);
    // }
}

public class WordMistakeNote {
    public WordObject WordObject { get; set; }
    
    public WordMistake WordMistake { get; set; }
}