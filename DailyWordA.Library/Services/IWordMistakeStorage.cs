using DailyWordA.Library.Models;

namespace DailyWordA.Library.Services;

public interface IWordMistakeStorage {
    bool IsInitialized { get; }

    Task InitializeAsync();

    Task<WordMistake> GetMistakeAsync(int wordId);

    Task<IEnumerable<WordMistake>> GetMistakeListAsync();

    Task SaveMistakeAsync(WordMistake wordMistake);

    event EventHandler<MistakeStorageUpdatedEventArgs> Updated;
}

public class MistakeStorageUpdatedEventArgs : EventArgs {
    public WordMistake UpdatedMistake { get; }

    public MistakeStorageUpdatedEventArgs(WordMistake updatedMistake) {
        UpdatedMistake = updatedMistake;
    }
}