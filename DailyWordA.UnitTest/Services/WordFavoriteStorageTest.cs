using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using DailyWordA.UnitTest.Helpers;
using Moq;

namespace DailyWordA.UnitTest.Services;

public class WordFavoriteStorageTest : IDisposable {
    public WordFavoriteStorageTest() {
        WordFavoriteStorageHelper.RemoveDatabaseFile();
    }
    
    public void Dispose() => WordFavoriteStorageHelper.RemoveDatabaseFile();
    
    [Fact]
    public async Task IsInitialized_Default() {
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        preferenceStorageMock
            .Setup(p => p.Get(WordFavoriteStorageConstant.VersionKey, default(int)))
            .Returns(WordFavoriteStorageConstant.Version);
        var mockPreferenceStorage = preferenceStorageMock.Object;

        var favoriteStorage = new WordFavoriteStorage(mockPreferenceStorage);
        await favoriteStorage.InitializeAsync();
        
        Assert.True(favoriteStorage.IsInitialized);
        preferenceStorageMock.Verify(p => p.Get(WordFavoriteStorageConstant.VersionKey, default(int)),
            Times.Once);
        await favoriteStorage.CloseAsync();
    }
    
    [Fact]
    public async Task InitializeAsync_Default() {
        var favoriteStorage = new WordFavoriteStorage(GetEmptyPreferenceStorage());
        Assert.False(File.Exists(WordFavoriteStorage.WordFavoriteDbPath));

        await favoriteStorage.InitializeAsync();
        Assert.True(File.Exists(WordFavoriteStorage.WordFavoriteDbPath));

        await favoriteStorage.CloseAsync();
    }
    
    [Fact]
    public async Task SaveFavoriteAsync_GetFavoriteAsync_Default() {
        var updated = false;
        WordFavorite updatedFavorite = null;

        var favoriteStorage = new WordFavoriteStorage(GetEmptyPreferenceStorage());
        favoriteStorage.Updated += (_, args) => {
            updated = true;
            updatedFavorite = args.UpdatedFavorite;
        };
        await favoriteStorage.InitializeAsync();

        var favoriteToSave = new WordFavorite {
            WordId = 1, IsFavorite = true
        };
        await favoriteStorage.SaveFavoriteAsync(favoriteToSave);

        var favorite =
            await favoriteStorage.GetFavoriteAsync(favoriteToSave.WordId);
        Assert.Equal(favoriteToSave.WordId, favorite.WordId);
        Assert.Equal(favoriteToSave.IsFavorite, favorite.IsFavorite);
        Assert.NotEqual(default, favorite.Timestamp);
        Assert.True(updated);
        Assert.Same(favoriteToSave, updatedFavorite);

        await favoriteStorage.SaveFavoriteAsync(favoriteToSave);
        favorite =
            await favoriteStorage.GetFavoriteAsync(favoriteToSave.WordId);
        Assert.True(DateTime.Today < favorite.Timestamp);

        await favoriteStorage.CloseAsync();
    }
    
    [Fact]
    public async Task GetFavoriteListAsync_Default() {
        var favoriteStorage = new WordFavoriteStorage(GetEmptyPreferenceStorage());
        await favoriteStorage.InitializeAsync();

        var favoriteListToSave = new List<WordFavorite>();
        var random = new Random();
        for (var i = 1; i <= 5; i++) {
            favoriteListToSave.Add(new WordFavorite {
                WordId = i, IsFavorite = random.NextDouble() > 0.5
            });
            await Task.Delay(10);
        }

        var favoriteDictionary = favoriteListToSave.Where(p => p.IsFavorite)
            .ToDictionary(p => p.WordId, p => true);

        foreach (var favoriteToSave in favoriteListToSave) {
            await favoriteStorage.SaveFavoriteAsync(favoriteToSave);
        }

        var favoriteList = await favoriteStorage.GetFavoriteListAsync();
        Assert.Equal(favoriteDictionary.Count, favoriteList.Count());
        foreach (var favorite in favoriteList) {
            Assert.True(favoriteDictionary.ContainsKey(favorite.WordId));
        }

        await favoriteStorage.CloseAsync();
    }
    
    private static IPreferenceStorage GetEmptyPreferenceStorage() =>
        new Mock<IPreferenceStorage>().Object;
}