using DailyWordA.Library.Models;

namespace DailyWordA.Library.Services;

public interface IMottoFavoriteStorage {
    bool IsInitialized { get; }

    Task InitializeAsync();

    // Task<DailyMotto> GetFavoriteMottoAsync(string date, string translation);
    Task<DailyMotto> GetFavoriteMottoAsync(int favoriteMottoId);

    Task<IEnumerable<DailyMotto>> GetFavoriteMottoListAsync();

    Task InsertFavoriteMottoAsync(DailyMotto favoriteMotto);
    
    Task DeleteFavoriteMottoAsync(DailyMotto favoriteMotto);

    event EventHandler<FavoriteMottoStorageUpdatedEventArgs> Updated;
}

public class FavoriteMottoStorageUpdatedEventArgs : EventArgs {
    public DailyMotto UpdatedFavoriteMotto { get; }
    
    public bool IsFavorite { get; }

    public FavoriteMottoStorageUpdatedEventArgs(DailyMotto favoriteMotto, bool isFavorite) {
        UpdatedFavoriteMotto = favoriteMotto;
        IsFavorite = isFavorite;
    }
}