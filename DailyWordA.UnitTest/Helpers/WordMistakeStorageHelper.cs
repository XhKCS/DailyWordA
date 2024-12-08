using DailyWordA.Library.Helpers;
using DailyWordA.Library.Services;

namespace DailyWordA.UnitTest.Helpers;

public class WordMistakeStorageHelper {
    public static void RemoveDatabaseFile() {
        var keyPath = PathHelper.GetLocalFilePath(WordMistakeStorageConstant.VersionKey);
        File.Delete(keyPath);
        File.Delete(WordMistakeStorage.WordMistakeDbPath);
    }
}