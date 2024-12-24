using DailyWordA.Library.Helpers;
using DailyWordA.Library.Services;

namespace DailyWordA.UnitTest.Helpers;

public class MottoFavoriteStorageHelper {
    public static void RemoveDatabaseFile() {
        var keyPath = PathHelper.GetLocalFilePath(MottoFavoriteStorageConstant.VersionKey);
        File.Delete(keyPath);
        File.Delete(MottoFavoriteStorage.MottoFavoriteDbPath);
    }
}