using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using DailyWordA.UnitTest.Helpers;
using Moq;

namespace DailyWordA.UnitTest.Services;

public class WordMistakeStorageTest : IDisposable {
    public WordMistakeStorageTest() {
        WordMistakeStorageHelper.RemoveDatabaseFile();
    }
    
    public void Dispose() => WordMistakeStorageHelper.RemoveDatabaseFile();
    
    [Fact]
    public async Task IsInitialized_Default() {
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        preferenceStorageMock
            .Setup(p => p.Get(WordMistakeStorageConstant.VersionKey, default(int)))
            .Returns(WordMistakeStorageConstant.Version);
        var mockPreferenceStorage = preferenceStorageMock.Object;

        var mistakeStorage = new WordMistakeStorage(mockPreferenceStorage);
        await mistakeStorage.InitializeAsync();
        
        Assert.True(mistakeStorage.IsInitialized);
        preferenceStorageMock.Verify(p => p.Get(WordMistakeStorageConstant.VersionKey, default(int)),
            Times.Once);
        await mistakeStorage.CloseAsync();
    }
    
    [Fact]
    public async Task InitializeAsync_Default() {
        var mistakeStorage = new WordMistakeStorage(GetEmptyPreferenceStorage());
        Assert.False(File.Exists(WordMistakeStorage.WordMistakeDbPath));

        await mistakeStorage.InitializeAsync();
        Assert.True(File.Exists(WordMistakeStorage.WordMistakeDbPath));

        await mistakeStorage.CloseAsync();
    }
    
    [Fact]
    public async Task SaveFavoriteAsync_GetFavoriteAsync_Default() {
        var updated = false;
        WordMistake updatedMistake = null;

        var mistakeStorage = new WordMistakeStorage(GetEmptyPreferenceStorage());
        mistakeStorage.Updated += (_, args) => {
            updated = true;
            updatedMistake = args.UpdatedMistake;
        };
        await mistakeStorage.InitializeAsync();

        var mistakeToSave = new WordMistake {
            WordId = 1, IsInNote = true
        };
        await mistakeStorage.SaveMistakeAsync(mistakeToSave);

        var wordMistake =
            await mistakeStorage.GetMistakeAsync(mistakeToSave.WordId);
        Assert.Equal(mistakeToSave.WordId, wordMistake.WordId);
        Assert.Equal(mistakeToSave.IsInNote, wordMistake.IsInNote);
        Assert.NotEqual(default, wordMistake.Timestamp);
        Assert.True(updated);
        Assert.Same(mistakeToSave, updatedMistake);

        await mistakeStorage.SaveMistakeAsync(mistakeToSave);
        wordMistake =
            await mistakeStorage.GetMistakeAsync(mistakeToSave.WordId);
        Assert.True(DateTime.Today < wordMistake.Timestamp);

        await mistakeStorage.CloseAsync();
    }
    
    [Fact]
    public async Task GetMistakeListAsync_Default() {
        var mistakeStorage = new WordMistakeStorage(GetEmptyPreferenceStorage());
        await mistakeStorage.InitializeAsync();

        var mistakeListToSave = new List<WordMistake>();
        var random = new Random();
        for (var i = 1; i <= 5; i++) {
            mistakeListToSave.Add(new WordMistake {
                WordId = i, IsInNote = random.NextDouble() > 0.5
            });
            await Task.Delay(10);
        }

        var mistakeDictionary = mistakeListToSave.Where(p => p.IsInNote)
            .ToDictionary(p => p.WordId, p => true);

        foreach (var mistakeToSave in mistakeListToSave) {
            await mistakeStorage.SaveMistakeAsync(mistakeToSave);
        }

        var mistakeList = await mistakeStorage.GetMistakeListAsync();
        Assert.Equal(mistakeDictionary.Count, mistakeList.Count());
        foreach (var favorite in mistakeList) {
            Assert.True(mistakeDictionary.ContainsKey(favorite.WordId));
        }

        await mistakeStorage.CloseAsync();
    }
    
    private static IPreferenceStorage GetEmptyPreferenceStorage() =>
        new Mock<IPreferenceStorage>().Object;
}