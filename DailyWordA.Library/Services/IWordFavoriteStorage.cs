using DailyWordA.Library.Models;

namespace DailyWordA.Library.Services;

public interface IWordFavoriteStorage {
    bool IsInitialized { get; }

    Task InitializeAsync();

    Task<WordFavorite> GetFavoriteAsync(int wordId);

    Task<IEnumerable<WordFavorite>> GetFavoriteListAsync();

    Task SaveFavoriteAsync(WordFavorite favorite);

    event EventHandler<FavoriteStorageUpdatedEventArgs> Updated;
}

public class FavoriteStorageUpdatedEventArgs : EventArgs {
    public WordFavorite UpdatedFavorite { get; }

    public FavoriteStorageUpdatedEventArgs(WordFavorite favorite) {
        UpdatedFavorite = favorite;
    }
}