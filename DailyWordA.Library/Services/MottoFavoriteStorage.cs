using System.Linq.Expressions;
using DailyWordA.Library.Helpers;
using DailyWordA.Library.Models;
using SQLite;

namespace DailyWordA.Library.Services;

public class MottoFavoriteStorage : IMottoFavoriteStorage {
    public const string DbName = "mottofavoritedb.sqlite3";
    public static readonly string MottoFavoriteDbPath =
        PathHelper.GetLocalFilePath(DbName);
    
    public event EventHandler<FavoriteMottoStorageUpdatedEventArgs>? Updated;
    
    private SQLiteAsyncConnection _connection;
    private SQLiteAsyncConnection Connection =>
        _connection ??= new SQLiteAsyncConnection(MottoFavoriteDbPath);
    
    private readonly IPreferenceStorage _preferenceStorage;

    public MottoFavoriteStorage(IPreferenceStorage preferenceStorage) {
        _preferenceStorage = preferenceStorage;
    }
    
    public bool IsInitialized =>
        _preferenceStorage.Get(MottoFavoriteStorageConstant.VersionKey, default(int)) 
            == MottoFavoriteStorageConstant.Version &&
            File.Exists(MottoFavoriteDbPath);
    
    public async Task InitializeAsync() {
        Console.WriteLine("Initializing MottoFavoriteStorage...");
        await Connection.CreateTableAsync<DailyMotto>();
        _preferenceStorage.Set(MottoFavoriteStorageConstant.VersionKey,
            MottoFavoriteStorageConstant.Version);
    }

    public async Task<DailyMotto> GetFavoriteMottoAsync(int favoriteMottoId) {
        return await Connection.Table<DailyMotto>()
            .FirstOrDefaultAsync(p => p.Id == favoriteMottoId);
    }

    public async Task<IEnumerable<DailyMotto>> GetFavoriteMottoListAsync() {
        return await Connection.Table<DailyMotto>()
            .OrderByDescending(p => p.FTimestamp).ToListAsync();
    }

    public async Task InsertFavoriteMottoAsync(DailyMotto favoriteMotto) {
        favoriteMotto.FTimestamp = DateTime.Now;
        await Connection.InsertAsync(favoriteMotto);
        // 触发更新事件
        Updated?.Invoke(this, new FavoriteMottoStorageUpdatedEventArgs(favoriteMotto, true));
    }

    public async Task DeleteFavoriteMottoAsync(DailyMotto favoriteMotto) {
        await Connection.DeleteAsync(favoriteMotto);
        // 触发更新事件
        Updated?.Invoke(this, new FavoriteMottoStorageUpdatedEventArgs(favoriteMotto, false));
    }
    
    public async Task CloseAsync() {
        await Connection.CloseAsync();
    }
}

public static class MottoFavoriteStorageConstant {
    public const string VersionKey =
        nameof(MottoFavoriteStorageConstant) + "." + nameof(Version);

    public const int Version = 1;
}