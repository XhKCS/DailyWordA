using System.Linq.Expressions;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;
using DailyWordA.UnitTest.Helpers;
using Moq;

namespace DailyWordA.UnitTest.ViewModels;

public class WordQueryResultViewModelTest
{
    // public WordQueryResultViewModelTest() =>
    //     WordStorageHelper.RemoveDatabaseFile();

    // public void Dispose() => WordStorageHelper.RemoveDatabaseFile();

    [Fact]
    public async Task SetParameter_WrongType() {
        var wordStorage =
            await WordStorageHelper.GetInitializedWordStorage();
        var contentNavigationServiceMock =
            new Mock<IContentNavigationService>();
        var mockContentNavigationService =
            contentNavigationServiceMock.Object;
        
        var resultViewModel = new WordQueryResultViewModel(wordStorage, mockContentNavigationService);
        var wrongParameter = "Wrong Parameter";
        resultViewModel.SetParameter(wrongParameter);
        Assert.Null(resultViewModel._where);
    }
    
    [Fact]
    public async Task WordCollection_Default() {
        var where = Expression.Lambda<Func<WordObject, bool>>(
            Expression.Constant(true),
            Expression.Parameter(typeof(WordObject), "p"));

        var wordStorage =
            await WordStorageHelper.GetInitializedWordStorage();
        var contentNavigationServiceMock =
            new Mock<IContentNavigationService>();
        var mockContentNavigationService =
            contentNavigationServiceMock.Object;
        
        var resultViewModel = new WordQueryResultViewModel(wordStorage, mockContentNavigationService);
        resultViewModel.SetParameter(where);

        var statusList = new List<string>();
        resultViewModel.PropertyChanged += (sender, args) => {
            if (args.PropertyName == nameof(resultViewModel.Status)) {
                statusList.Add(resultViewModel.Status);
            }
        };

        Assert.Empty(resultViewModel.WordCollection);
        await resultViewModel.WordCollection.LoadMoreAsync();
        Assert.Equal(50, resultViewModel.WordCollection.Count);
        Assert.Equal("steer",
            resultViewModel.WordCollection.Last().Word);
        Assert.True(resultViewModel.WordCollection.CanLoadMore);
        Assert.Equal(2, statusList.Count);
        Assert.Equal(WordQueryResultViewModel.Loading, statusList[0]);
        Assert.Equal("", statusList[1]);

        var wordCollectionChanged = false;
        resultViewModel.WordCollection.CollectionChanged += (sender, args) => wordCollectionChanged = true;
        await resultViewModel.WordCollection.LoadMoreAsync();
        Assert.True(wordCollectionChanged);
        Assert.Equal(100, resultViewModel.WordCollection.Count);
        Assert.Equal("tag",
            resultViewModel.WordCollection[99].Word);

        await wordStorage.CloseAsync();
    }
    
    [Fact]
    public void ShowDetail_Default() {
        var contentNavigationServiceMock =
            new Mock<IContentNavigationService>();
        var mockContentNavigationService =
            contentNavigationServiceMock.Object;

        var wordToTap = new WordObject();
        var resultViewModel =
            new WordQueryResultViewModel(null, mockContentNavigationService);
        resultViewModel.ShowWordDetail(wordToTap);
        contentNavigationServiceMock.Verify(
            p => p.NavigateTo(ContentNavigationConstant.WordDetailView, wordToTap), Times.Once);
    }
}