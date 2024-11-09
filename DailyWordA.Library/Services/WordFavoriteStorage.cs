using DailyWordA.Library.Helpers;
using DailyWordA.Library.Models;
using SQLite;

namespace DailyWordA.Library.Services;

public class WordFavoriteStorage : IWordFavoriteStorage {
    public const string DbName = "wordfavoritedb.sqlite3";
    public static readonly string WordFavoriteDbPath =
        PathHelper.GetLocalFilePath(DbName);
    
    public event EventHandler<FavoriteStorageUpdatedEventArgs>? Updated;
    
    private SQLiteAsyncConnection _connection;
    private SQLiteAsyncConnection Connection =>
        _connection ??= new SQLiteAsyncConnection(WordFavoriteDbPath);
    
    private readonly IPreferenceStorage _preferenceStorage;

    public WordFavoriteStorage(IPreferenceStorage preferenceStorage) {
        _preferenceStorage = preferenceStorage;
    }
    
    public bool IsInitialized =>
        _preferenceStorage.Get(WordFavoriteStorageConstant.VersionKey,
            default(int)) == WordFavoriteStorageConstant.Version;
    
    public async Task InitializeAsync() {
        Console.WriteLine("Initializing WordFavoriteStorage...");
        await Connection.CreateTableAsync<WordFavorite>();
        _preferenceStorage.Set(WordFavoriteStorageConstant.VersionKey,
            WordFavoriteStorageConstant.Version);
    }

    public async Task<WordFavorite> GetFavoriteAsync(int wordId) {
        return await Connection.Table<WordFavorite>()
            .FirstOrDefaultAsync(p => p.WordId == wordId);
    }

    public async Task<IEnumerable<WordFavorite>> GetFavoriteListAsync() {
        return await Connection.Table<WordFavorite>().Where(p => p.IsFavorite)
            .OrderByDescending(p => p.Timestamp).ToListAsync();
    }

    public async Task SaveFavoriteAsync(WordFavorite favorite) {
        favorite.Timestamp = DateTime.Now;
        await Connection.InsertOrReplaceAsync(favorite);
        // 触发更新事件
        Updated?.Invoke(this, new FavoriteStorageUpdatedEventArgs(favorite));
    }

    public async Task CloseAsync() {
        await Connection.CloseAsync();
    }
    
}

public static class WordFavoriteStorageConstant {
    public const string VersionKey =
        nameof(WordFavoriteStorageConstant) + "." + nameof(Version);

    public const int Version = 1;
}