using DailyWordA.Library.Helpers;
using DailyWordA.Library.Services;

namespace DailyWordA.UnitTest.Helpers;

public class WordFavoriteStorageHelper {
    public static void RemoveDatabaseFile() {
        var keyPath = PathHelper.GetLocalFilePath(WordFavoriteStorageConstant.VersionKey);
        File.Delete(keyPath);
        File.Delete(WordFavoriteStorage.WordFavoriteDbPath);
    }
}