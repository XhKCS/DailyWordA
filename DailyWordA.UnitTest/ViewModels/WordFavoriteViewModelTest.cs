using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;
using Moq;

namespace DailyWordA.UnitTest.ViewModels;

public class WordFavoriteViewModelTest {
    [Fact]
    public async Task LoadedCommandFunction_Default() {
        var wordListToReturn = new List<WordObject>();
        for (var i = 1; i <= 5; i++) {
            wordListToReturn.Add(new WordObject {
                Id = i
            });
        }

        var favoriteListToReturn = new List<WordFavorite>();
        favoriteListToReturn.AddRange(
            wordListToReturn.Select(p => new WordFavorite {
                WordId = p.Id
            }));

        var favoriteStorageMock = new Mock<IWordFavoriteStorage>();
        favoriteStorageMock.Setup(p => p.GetFavoriteListAsync())
            .ReturnsAsync(favoriteListToReturn);
        var mockFavoriteStorage = favoriteStorageMock.Object;

        var wordStorageMock = new Mock<IWordStorage>();
        wordListToReturn.ForEach(p =>
            wordStorageMock.Setup(m => m.GetWordAsync(p.Id))
                .ReturnsAsync(p));
        var mockWordStorage = wordStorageMock.Object;

        var favoritePageViewModel =
            new WordFavoriteViewModel(mockFavoriteStorage, mockWordStorage,
                null);

        var loadingList = new List<bool>();
        favoritePageViewModel.PropertyChanged += (sender, args) => {
            if (args.PropertyName == nameof(WordFavoriteViewModel.IsLoading)) {
                loadingList.Add(favoritePageViewModel.IsLoading);
            }
        };

        Assert.Empty(favoritePageViewModel.WordFavoriteCollection);
        await favoritePageViewModel.OnInitializedAsync();
        Assert.Equal(favoriteListToReturn.Count,
            favoritePageViewModel.WordFavoriteCollection.Count);
        favoriteStorageMock.Verify(p => p.GetFavoriteListAsync(), Times.Once);
        favoriteListToReturn.ForEach(p =>
            wordStorageMock.Verify(m => m.GetWordAsync(p.WordId),
                Times.Once));

        for (var i = 0;
             i < favoritePageViewModel.WordFavoriteCollection.Count;
             i++) {
            Assert.Same(favoriteListToReturn[i],
                favoritePageViewModel.WordFavoriteCollection[i].Favorite);
            Assert.Same(wordListToReturn[i],
                favoritePageViewModel.WordFavoriteCollection[i].WordObject);
        }
    }
    
    [Fact]
    public async Task WordTappedCommandFunction_Default() {
        var contentNavigationServiceMock =
            new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;

        var favoriteStorageMock = new Mock<IWordFavoriteStorage>();
        var mockFavoriteStorage = favoriteStorageMock.Object;

        var favoritePageViewModel =
            new WordFavoriteViewModel(mockFavoriteStorage, null,
                mockContentNavigationService);
        var wordFavoriteToNavigate =
            new WordFavoriteCombination {
                WordObject = new WordObject()
            };

        favoritePageViewModel.ShowWordDetail(
            wordFavoriteToNavigate.WordObject);
        contentNavigationServiceMock.Verify(
            p => p.NavigateTo(ContentNavigationConstant.WordDetailView,
                wordFavoriteToNavigate.WordObject), Times.Once);
    }
    
    [Fact]
    public void FavoriteStorageOnUpdated_Default() {
        var favoriteStorageMock = new Mock<IWordFavoriteStorage>();
        var mockFavoriteStorage = favoriteStorageMock.Object;

        var wordFavoriteList = new List<WordFavoriteCombination>();
        for (int i = 1; i <= 5; i++) {
            wordFavoriteList.Add(new WordFavoriteCombination {
                Favorite = new WordFavorite {
                    WordId = i, IsFavorite = true, Timestamp = DateTime.Now.Subtract(TimeSpan.FromMinutes(i))
                }
            });
        }

        var favoriteUpdated = new WordFavorite {
            WordId = wordFavoriteList[2].Favorite.WordId,
            IsFavorite = false,
            Timestamp = wordFavoriteList[2].Favorite.Timestamp
        };
        var wordToReturn = new WordObject {
            Id = favoriteUpdated.WordId
        };

        var wordStorageMock = new Mock<IWordStorage>();
        wordStorageMock.Setup(p => p.GetWordAsync(wordToReturn.Id))
            .ReturnsAsync(wordToReturn);
        var mockWordStorage = wordStorageMock.Object;

        var favoritePageViewModel =
            new WordFavoriteViewModel(mockFavoriteStorage, mockWordStorage,
                null);
        Assert.Empty(favoritePageViewModel.WordFavoriteCollection);
        favoritePageViewModel.WordFavoriteCollection.AddRange(
            wordFavoriteList);

        favoriteStorageMock.Raise(p => p.Updated += null, mockFavoriteStorage,
            new WordFavoriteStorageUpdatedEventArgs(favoriteUpdated));
        Assert.Equal(wordFavoriteList.Count - 1,
            favoritePageViewModel.WordFavoriteCollection.Count);
        Assert.DoesNotContain(favoritePageViewModel.WordFavoriteCollection, p =>
            p.Favorite.WordId == favoriteUpdated.WordId);

        favoriteUpdated.IsFavorite = true;
        favoriteStorageMock.Raise(p => p.Updated += null, mockFavoriteStorage,
            new WordFavoriteStorageUpdatedEventArgs(favoriteUpdated));
        Assert.Equal(wordFavoriteList.Count,
            favoritePageViewModel.WordFavoriteCollection.Count);
        Assert.Equal(favoriteUpdated.WordId - 1,
            favoritePageViewModel.WordFavoriteCollection.IndexOf(
                favoritePageViewModel.WordFavoriteCollection.First(p =>
                    p.Favorite.WordId == favoriteUpdated.WordId)));
    }
}