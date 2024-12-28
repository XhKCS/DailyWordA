using System.Linq.Expressions;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using DailyWordA.UnitTest.Helpers;
using Moq;

namespace DailyWordA.UnitTest.Services;

public class WordStorageTest : IDisposable {
    public WordStorageTest() {
        // WordStorageHelper.RemoveDatabaseFile();
    }

    public void Dispose()
    {
        
    }
    
    [Fact]
    public async Task IsInitialized_Default() {
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        preferenceStorageMock.Setup(p => p.Get(WordStorageConstant.VersionKey, default(int)))
            .Returns(WordStorageConstant.Version);
        var mockPreferenceStorage = preferenceStorageMock.Object;
        var alertStorageMock = new Mock<IAlertService>();
        var mockAlertService = alertStorageMock.Object;
        var wordStorage = new WordStorage(mockPreferenceStorage, mockAlertService);
        
        await wordStorage.InitializeAsync();
        Assert.True(wordStorage.IsInitialized);
        preferenceStorageMock.Verify(p => p.Get(WordStorageConstant.VersionKey, default(int)),
            Times.AtLeastOnce);
        await wordStorage.CloseAsync();
    }
    
    [Fact]
    public async Task InitializeAsync_Default() {
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        var mockPreferenceStorage = preferenceStorageMock.Object;
        var alertStorageMock = new Mock<IAlertService>();
        var mockAlertService = alertStorageMock.Object;
        
        var wordStorage = new WordStorage(mockPreferenceStorage, mockAlertService);
        
        await wordStorage.InitializeAsync();
        Assert.True(File.Exists(WordStorage.WordDbPath));
        
        preferenceStorageMock.Verify(
            p => p.Set(WordStorageConstant.VersionKey, WordStorageConstant.Version), 
            Times.Once);
    }

    [Fact]
    public async Task GetWordAsync_Default() {
        var wordStorage = await WordStorageHelper.GetInitializedWordStorage();
        var wordObject = await wordStorage.GetWordAsync(5003);
        Assert.Equal("confidential", wordObject.Word);
        await wordStorage.CloseAsync();
    }

    [Fact]
    public async Task GetRandomWordAsync_Default() {
        var wordStorage = await WordStorageHelper.GetInitializedWordStorage();
        var wordObject = await wordStorage.GetRandomWordAsync();
        Assert.NotNull(wordObject);
        Assert.True(wordObject.Word.Length > 0);
        Assert.True(wordObject.Id > 0);
        await wordStorage.CloseAsync();
    }
    
    [Fact]
    public async Task GetWordsAsync_Default() {
        var wordStorage = await WordStorageHelper.GetInitializedWordStorage();
        var wordList = await wordStorage.GetWordsAsync(
            Expression.Lambda<Func<WordObject, bool>>(Expression.Constant(true),
                Expression.Parameter(typeof(WordObject), "p")), 0, int.MaxValue);
        Assert.Equal(WordStorage.NumberOfWords, wordList.Count);
        await wordStorage.CloseAsync();
    }

    [Fact]
    public async Task GetWordQuizOptionsAsync_Default() {
        var wordStorage = await WordStorageHelper.GetInitializedWordStorage();
        var wordObject = await wordStorage.GetRandomWordAsync();
        var wordQuizOptions = await wordStorage.GetWordQuizOptionsAsync(wordObject);
        Assert.Equal(4, wordQuizOptions.Count);
        Assert.Distinct(wordQuizOptions);
        Assert.Contains(wordObject, wordQuizOptions);
        await wordStorage.CloseAsync();
    }
}