using DailyWordA.Library.Helpers;
using DailyWordA.Library.Services;
using Moq;

namespace DailyWordA.UnitTest.Helpers;

public class WordStorageHelper {
    public static void RemoveDatabaseFile() {
        var keyPath = PathHelper.GetLocalFilePath(WordFavoriteStorageConstant.VersionKey);
        File.Delete(keyPath);
        File.Delete(WordStorage.WordDbPath);
    }
    
    public static async Task<WordStorage> GetInitializedWordStorage() {
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        preferenceStorageMock.Setup(p =>
            p.Get(WordStorageConstant.VersionKey, -1)).Returns(-1);
        var mockPreferenceStorage = preferenceStorageMock.Object;
        var alertStorageMock = new Mock<IAlertService>();
        var mockAlertService = alertStorageMock.Object;
        var wordStorage = new WordStorage(mockPreferenceStorage, mockAlertService);
        await wordStorage.InitializeAsync();
        return wordStorage;
    }
}