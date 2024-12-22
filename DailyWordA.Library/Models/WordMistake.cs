using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace DailyWordA.Library.Models;

public class WordMistake  : ObservableObject {
    [PrimaryKey] public int WordId { get; set; }
    
    private bool _isInNote;
    public virtual bool IsInNote {
        get => _isInNote;
        set => SetProperty(ref _isInNote, value);
    }
    
    public DateTime Timestamp { get; set; }
    
    public override bool Equals(object obj)
    {
        if (obj is WordMistake wordMistake) {
            if (wordMistake.WordId == WordId && 
                wordMistake.IsInNote == IsInNote) {
                return true;
            }
        }
        return false;
    }
}