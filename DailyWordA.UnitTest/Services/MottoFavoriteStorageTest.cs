using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using DailyWordA.UnitTest.Helpers;
using Moq;

namespace DailyWordA.UnitTest.Services;

public class MottoFavoriteStorageTest : IDisposable {
    public void Dispose() => MottoFavoriteStorageHelper.RemoveDatabaseFile();
    
    [Fact]
    public async Task IsInitialized_Default() {
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        preferenceStorageMock
            .Setup(p => p.Get(MottoFavoriteStorageConstant.VersionKey, default(int)))
            .Returns(MottoFavoriteStorageConstant.Version);
        var mockPreferenceStorage = preferenceStorageMock.Object;

        var favoriteStorage = new MottoFavoriteStorage(mockPreferenceStorage);
        await favoriteStorage.InitializeAsync();
        
        Assert.True(favoriteStorage.IsInitialized);
        preferenceStorageMock.Verify(p => p.Get(MottoFavoriteStorageConstant.VersionKey, default(int)),
            Times.Once);
        await favoriteStorage.CloseAsync();
    }
    
    [Fact]
    public async Task InitializeAsync_Default() {
        var favoriteStorage = new MottoFavoriteStorage(GetEmptyPreferenceStorage());

        await favoriteStorage.InitializeAsync();
        Assert.True(File.Exists(MottoFavoriteStorage.MottoFavoriteDbPath));

        await favoriteStorage.CloseAsync();
    }
    
    [Fact]
    public async Task InsertOrDeleteFavoriteMottoAsync_GetFavoriteMottoAsync_Default() {
        var updated = false;
        var isFavorite = false;
        DailyMotto updatedMotto = null;

        var favoriteStorage = new MottoFavoriteStorage(GetEmptyPreferenceStorage());
        favoriteStorage.Updated += (_, args) => {
            updated = true;
            isFavorite = args.IsFavorite;
            updatedMotto = args.UpdatedFavoriteMotto;
        };
        await favoriteStorage.InitializeAsync();

        var favoriteMottoToSave = new DailyMotto {
            Id = 1, Date = "2024-12-23"
        };
        await favoriteStorage.InsertFavoriteMottoAsync(favoriteMottoToSave);

        var favoriteMotto =
            await favoriteStorage.GetFavoriteMottoAsync(favoriteMottoToSave.Id);
        Assert.Equal(favoriteMottoToSave.Id, favoriteMotto.Id);
        Assert.NotEqual(default, favoriteMotto.FTimestamp);
        Assert.True(updated);
        Assert.True(isFavorite);
        Assert.Same(favoriteMottoToSave, updatedMotto);
        
        await favoriteStorage.DeleteFavoriteMottoAsync(favoriteMottoToSave);
        Assert.False(isFavorite);

        await favoriteStorage.CloseAsync();
    }
    
    [Fact]
    public async Task GetFavoriteListAsync_Default() {
        var favoriteStorage = new MottoFavoriteStorage(GetEmptyPreferenceStorage());
        await favoriteStorage.InitializeAsync();

        var favoriteListToSave = new List<DailyMotto>();
        var random = new Random();
        for (var i = 1; i <= 5; i++) {
            favoriteListToSave.Add(new DailyMotto {
                Id = i
            });
            await Task.Delay(10);
        }
        
        var favoriteDictionary = favoriteListToSave.ToDictionary(p => p.Id, p => true);
        foreach (var favoriteToSave in favoriteListToSave) {
            await favoriteStorage.InsertFavoriteMottoAsync(favoriteToSave);
        }

        var favoriteList = await favoriteStorage.GetFavoriteMottoListAsync();
        foreach (var favorite in favoriteList) {
            Assert.True(favoriteDictionary.ContainsKey(favorite.Id));
        }

        await favoriteStorage.CloseAsync();
    }
    
    private static IPreferenceStorage GetEmptyPreferenceStorage() =>
        new Mock<IPreferenceStorage>().Object;
}