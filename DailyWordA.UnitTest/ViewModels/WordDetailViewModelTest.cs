using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;
using DailyWordA.UnitTest.Helpers;
using Moq;

namespace DailyWordA.UnitTest.ViewModels;

public class WordDetailViewModelTest {
    [Fact]
    public async Task OnLoadedAsync_Default() {
        var wordObject = new WordObject {
            Id = 1
        };
        
        var favoriteToReturn =
            new WordFavorite {
                WordId = wordObject.Id, IsFavorite = true
            };
        var favoriteStorageMock = new Mock<IWordFavoriteStorage>();
        favoriteStorageMock
            .Setup(p => p.GetFavoriteAsync(favoriteToReturn.WordId))
            .ReturnsAsync(favoriteToReturn);
        var mockFavoriteStorage = favoriteStorageMock.Object;
        
        var mistakeToReturn =
            new WordMistake {
                WordId = wordObject.Id, IsInNote = true
            };
        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        mistakeStorageMock
            .Setup(p => p.GetMistakeAsync(favoriteToReturn.WordId))
            .ReturnsAsync(mistakeToReturn);
        var mockMistakeStorage = mistakeStorageMock.Object;

        var wordDetailViewModel = new WordDetailViewModel(null, mockFavoriteStorage, null, mockMistakeStorage);
        wordDetailViewModel.SetParameter(wordObject);
        Assert.Same(wordObject, wordDetailViewModel.CurrentWord);

        var loadingList = new List<bool>();
        wordDetailViewModel.PropertyChanged += (sender, args) => {
            if (args.PropertyName == nameof(WordDetailViewModel.IsLoading)) {
                loadingList.Add(wordDetailViewModel.IsLoading);
            }
        };

        await wordDetailViewModel.OnLoadedAsync();

        favoriteStorageMock.Verify(
            p => p.GetFavoriteAsync(favoriteToReturn.WordId), Times.Once);
        Assert.Same(favoriteToReturn, wordDetailViewModel.Favorite);
        mistakeStorageMock.Verify(
            p => p.GetMistakeAsync(favoriteToReturn.WordId), Times.Once);
        Assert.Same(mistakeToReturn, wordDetailViewModel.Mistake);
        Assert.Equal(2, loadingList.Count);
        Assert.True(loadingList.First());
        Assert.False(loadingList.Last());

        await wordDetailViewModel.FavoriteSwitchClickedAsync();
        favoriteStorageMock.Verify(
            p => p.SaveFavoriteAsync(favoriteToReturn), Times.Once);
        
        await wordDetailViewModel.MistakeSwitchClickedAsync();
        mistakeStorageMock.Verify(
            p => p.SaveMistakeAsync(mistakeToReturn), Times.Once);
    }
    
    [Fact]
    public async Task Query_Default() {
        object parameter = null;
        var menuNavigationServiceMock = new Mock<IMenuNavigationService>();
        menuNavigationServiceMock
            .Setup(p => p.NavigateTo(MenuNavigationConstant.WordQueryView,
                It.IsAny<object>()))
            .Callback<string, object>((s, o) => parameter = o);
        var mockMenuNavigationService = menuNavigationServiceMock.Object;

        var wordStorage = await WordStorageHelper.GetInitializedWordStorage();
        
        var currentWord = await wordStorage.GetWordAsync(5003);
        var wordDetailViewModel =
            new WordDetailViewModel(mockMenuNavigationService, null, null, null);
        wordDetailViewModel.SetParameter(currentWord);
        
        wordDetailViewModel.Query();
        
        menuNavigationServiceMock.Verify(p => p.NavigateTo(
            MenuNavigationConstant.WordQueryView, parameter), Times.Once);
        Assert.IsType<WordQuery>(parameter);
        var wordQuery = (WordQuery)parameter;
        Assert.Equal(currentWord.Word, wordQuery.Word);
        Assert.Equal(currentWord.CnMeaning, wordQuery.CnMeaning);
    }
}