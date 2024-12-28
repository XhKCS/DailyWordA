using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;
using Moq;

namespace DailyWordA.UnitTest.ViewModels;

public class MottoDetailViewModelTest {
    [Fact]
    public void SetParameter_Default() {
        var todayMotto = new DailyMotto {
            Content = "Once you surrender to your vision, success begins to chase you."
        };
        
        var todayMottoViewModel = new MottoDetailViewModel(null);
        todayMottoViewModel.SetParameter(todayMotto);
        Assert.Equal(todayMottoViewModel.CurrentMotto, todayMotto);
    }
    
    [Fact]
    public async Task OnLoadedAsync_Default() {
        
        var favoriteMottoToReturn =
            new DailyMotto {
                Id = 1
            };
        var favoriteStorageMock = new Mock<IMottoFavoriteStorage>();
        favoriteStorageMock
            .Setup(p => p.GetFavoriteMottoAsync(favoriteMottoToReturn.Id))
            .ReturnsAsync(favoriteMottoToReturn);
        var mockFavoriteStorage = favoriteStorageMock.Object;

        var mottoDetailViewModel = new MottoDetailViewModel(mockFavoriteStorage);
        mottoDetailViewModel.SetParameter(favoriteMottoToReturn);
        Assert.Same(favoriteMottoToReturn, mottoDetailViewModel.CurrentMotto);

        var loadingList = new List<bool>();
        mottoDetailViewModel.PropertyChanged += (sender, args) => {
            if (args.PropertyName == nameof(MottoDetailViewModel.IsLoading)) {
                loadingList.Add(mottoDetailViewModel.IsLoading);
            }
        };

        await mottoDetailViewModel.OnLoadedAsync();

        favoriteStorageMock.Verify(
            p => p.GetFavoriteMottoAsync(favoriteMottoToReturn.Id), Times.Once);
        Assert.True(mottoDetailViewModel.IsFavorite);
        Assert.Equal(2, loadingList.Count);

        await mottoDetailViewModel.FavoriteSwitchClickedAsync();
        favoriteStorageMock.Verify(
            p => p.InsertFavoriteMottoAsync(favoriteMottoToReturn), Times.Once);
        
        mottoDetailViewModel.IsFavorite = false;
        await mottoDetailViewModel.FavoriteSwitchClickedAsync();
        favoriteStorageMock.Verify(
            p => p.DeleteFavoriteMottoAsync(favoriteMottoToReturn), Times.Once);
    }
}