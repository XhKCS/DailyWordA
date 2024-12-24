using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;
using Moq;

namespace DailyWordA.UnitTest.ViewModels;

public class MottoFavoriteViewModelTest {
    [Fact]
    public async Task OnInitializedCommandFunction_Default() {
        var favoriteMottoListToReturn = new List<DailyMotto>();
        for (var i = 1; i <= 5; i++) {
            favoriteMottoListToReturn.Add(new DailyMotto {
                Id = i
            });
        }

        var favoriteStorageMock = new Mock<IMottoFavoriteStorage>();
        favoriteStorageMock.Setup(p => p.GetFavoriteMottoListAsync())
            .ReturnsAsync(favoriteMottoListToReturn);
        var mockFavoriteStorage = favoriteStorageMock.Object;
        
        var mottoFavoriteViewModel =
            new MottoFavoriteViewModel(mockFavoriteStorage, null);

        Assert.Empty(mottoFavoriteViewModel.FavoriteMottoCollection);
        await mottoFavoriteViewModel.OnInitializedAsync();
        Assert.Equal(favoriteMottoListToReturn.Count,
            mottoFavoriteViewModel.FavoriteMottoCollection.Count);
        favoriteStorageMock.Verify(p => p.GetFavoriteMottoListAsync(), Times.Once);

        for (var i = 0;
             i < mottoFavoriteViewModel.FavoriteMottoCollection.Count;
             i++) {
            Assert.Same(favoriteMottoListToReturn[i],
                mottoFavoriteViewModel.FavoriteMottoCollection[i]);
        }
    }
    
    [Fact]
    public async Task MottoTappedCommandFunction_Default() {
        var contentNavigationServiceMock =
            new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;

        var favoriteStorageMock = new Mock<IMottoFavoriteStorage>();
        var mockFavoriteStorage = favoriteStorageMock.Object;

        var mottoFavoriteViewModel = new MottoFavoriteViewModel(mockFavoriteStorage, mockContentNavigationService);
        var mottoToNavigate = new DailyMotto { Id = 1 };

        mottoFavoriteViewModel.ShowMottoDetail(mottoToNavigate);
        contentNavigationServiceMock.Verify(
            p => p.NavigateTo(ContentNavigationConstant.MottoDetailView, 
                mottoToNavigate), Times.Once);
    }
    
    [Fact]
    public void FavoriteStorageOnUpdated_Default() {
        var favoriteMottoList = new List<DailyMotto>();
        for (int i = 1; i <= 5; i++) {
            favoriteMottoList.Add(new DailyMotto {Id = i, 
                FTimestamp = DateTime.Now.Subtract(TimeSpan.FromMinutes(i))});
        }

        var favoriteMottoUpdated = favoriteMottoList[4];
        
        var contentNavigationServiceMock =
            new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;

        var mottoFavoriteStorageMock = new Mock<IMottoFavoriteStorage>();
        mottoFavoriteStorageMock.Setup(p => p.GetFavoriteMottoAsync(favoriteMottoUpdated.Id))
            .ReturnsAsync(favoriteMottoUpdated);
        // mottoFavoriteStorageMock.Setup(p => p.GetFavoriteMottoListAsync())
        //     .ReturnsAsync(favoriteMottoList);
        var mockMottoFavoriteStorage = mottoFavoriteStorageMock.Object;

        var mottoFavoriteViewModel = new MottoFavoriteViewModel(mockMottoFavoriteStorage, mockContentNavigationService);
        Assert.Empty(mottoFavoriteViewModel.FavoriteMottoCollection);
        mottoFavoriteViewModel.FavoriteMottoCollection.AddRange(
            favoriteMottoList);

        mottoFavoriteStorageMock.Raise(p => p.Updated += null, mockMottoFavoriteStorage, 
            new FavoriteMottoStorageUpdatedEventArgs(favoriteMottoUpdated, false));
        Assert.Equal(favoriteMottoList.Count -1, mottoFavoriteViewModel.FavoriteMottoCollection.Count);
        Assert.DoesNotContain(mottoFavoriteViewModel.FavoriteMottoCollection, p => p.Id == favoriteMottoUpdated.Id);
        
        mottoFavoriteStorageMock.Raise(p => p.Updated += null, mockMottoFavoriteStorage, 
            new FavoriteMottoStorageUpdatedEventArgs(favoriteMottoUpdated, true));
        Assert.Equal(favoriteMottoList.Count, mottoFavoriteViewModel.FavoriteMottoCollection.Count);
        Assert.Contains(favoriteMottoUpdated, mottoFavoriteViewModel.FavoriteMottoCollection);
    }
}