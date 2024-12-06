using DailyWordA.Library.Helpers;
using DailyWordA.Library.Models;
using SQLite;

namespace DailyWordA.Library.Services;

public class WordMistakeStorage : IWordMistakeStorage {
    public const string DbName = "wordmistakedb.sqlite3";
    public static readonly string WordMistakeDbPath =
        PathHelper.GetLocalFilePath(DbName);
    
    public event EventHandler<MistakeStorageUpdatedEventArgs>? Updated;
    
    private SQLiteAsyncConnection _connection;
    private SQLiteAsyncConnection Connection =>
        _connection ??= new SQLiteAsyncConnection(WordMistakeDbPath);
    
    private readonly IPreferenceStorage _preferenceStorage;

    public WordMistakeStorage(IPreferenceStorage preferenceStorage) {
        _preferenceStorage = preferenceStorage;
    }
    
    public bool IsInitialized =>
        _preferenceStorage.Get(WordMistakeStorageConstant.VersionKey, default(int)) 
            == WordMistakeStorageConstant.Version &&
            File.Exists(WordMistakeDbPath);
    
    public async Task InitializeAsync() {
        await Connection.CreateTableAsync<WordMistake>();
        _preferenceStorage.Set(WordMistakeStorageConstant.VersionKey,
            WordMistakeStorageConstant.Version);
    }

    public async Task<WordMistake> GetMistakeAsync(int wordId) {
        return await Connection.Table<WordMistake>()
            .FirstOrDefaultAsync(p => p.WordId == wordId);
    }

    public async Task<IEnumerable<WordMistake>> GetMistakeListAsync() {
        return await Connection.Table<WordMistake>().Where(p => p.IsInNote)
            .OrderByDescending(p => p.Timestamp).ToListAsync();
    }

    public async Task SaveMistakeAsync(WordMistake wordMistake) {
        wordMistake.Timestamp = DateTime.Now;
        await Connection.InsertOrReplaceAsync(wordMistake);
        // 触发更新事件
        Updated?.Invoke(this, new MistakeStorageUpdatedEventArgs(wordMistake));
    }
    
    public async Task CloseAsync() {
        await Connection.CloseAsync();
    }
}

public static class WordMistakeStorageConstant {
    public const string VersionKey =
        nameof(WordMistakeStorageConstant) + "." + nameof(Version);

    public const int Version = 1;
}