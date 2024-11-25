using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;
using Moq;

namespace DailyWordA.UnitTest.ViewModels;

public class InitializationViewModelTest {
    [Fact]
    public async Task OnInitializedAsync_NotInitialized() {
        var wordStorageMock = new Mock<IWordStorage>();
        wordStorageMock.Setup(p => p.IsInitialized).Returns(false);
        var mockWordStorage = wordStorageMock.Object;

        var favoriteStorageMock = new Mock<IWordFavoriteStorage>();
        favoriteStorageMock.Setup(p => p.IsInitialized).Returns(false);
        var mockFavoriteStorage = favoriteStorageMock.Object;

        var rootNavigationServiceMock = new Mock<IRootNavigationService>();
        var mockRootNavigationService = rootNavigationServiceMock.Object;
        
        var initializationViewModel = new InitializationViewModel(
            mockWordStorage, mockRootNavigationService, mockFavoriteStorage);

        await initializationViewModel.OnInitializedAsync();
        wordStorageMock.Verify(p => p.IsInitialized, Times.Once);
        wordStorageMock.Verify(p => p.InitializeAsync(), Times.Once);
        favoriteStorageMock.Verify(p => p.IsInitialized, Times.Once);
        favoriteStorageMock.Verify(p => p.InitializeAsync(), Times.Once);
        rootNavigationServiceMock.Verify(
            p => p.NavigateTo(RootNavigationConstant.MainView), Times.Once);
    }

    [Fact]
    public async Task OnInitializedAsync_Initialized() {
        var wordStorageMock = new Mock<IWordStorage>();
        wordStorageMock.Setup(p => p.IsInitialized).Returns(true);
        var mockWordStorage = wordStorageMock.Object;

        var favoriteStorageMock = new Mock<IWordFavoriteStorage>();
        favoriteStorageMock.Setup(p => p.IsInitialized).Returns(true);
        var mockFavoriteStorage = favoriteStorageMock.Object;

        var rootNavigationServiceMock = new Mock<IRootNavigationService>();
        var mockRootNavigationService = rootNavigationServiceMock.Object;
        
        var initializationViewModel = new InitializationViewModel(
            mockWordStorage, mockRootNavigationService, mockFavoriteStorage);

        await initializationViewModel.OnInitializedAsync();
        wordStorageMock.Verify(p => p.IsInitialized, Times.Once);
        wordStorageMock.Verify(p => p.InitializeAsync(), Times.Never);
        favoriteStorageMock.Verify(p => p.IsInitialized, Times.Once);
        favoriteStorageMock.Verify(p => p.InitializeAsync(), Times.Never);
        rootNavigationServiceMock.Verify(
            p => p.NavigateTo(RootNavigationConstant.MainView), Times.Once);
    }
}