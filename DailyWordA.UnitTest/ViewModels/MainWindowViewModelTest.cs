using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;
using Moq;

namespace DailyWordA.UnitTest.ViewModels;

public class MainWindowViewModelTest {
    [Fact]
    public async Task OnInitializedAsync_NotInitialized() {
        var wordStorageMock = new Mock<IWordStorage>();
        wordStorageMock.Setup(p => p.IsInitialized).Returns(false);
        var mockWordStorage = wordStorageMock.Object;

        var wordFavoriteStorageMock = new Mock<IWordFavoriteStorage>();
        wordFavoriteStorageMock.Setup(p => p.IsInitialized).Returns(false);
        var mockFavoriteStorage = wordFavoriteStorageMock.Object;
        
        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        mistakeStorageMock.Setup(p => p.IsInitialized).Returns(false);
        var mockMistakeStorage = mistakeStorageMock.Object;
        
        var mottoFavoriteStorageMock = new Mock<IMottoFavoriteStorage>();
        mottoFavoriteStorageMock.Setup(p => p.IsInitialized).Returns(false);
        var mockFavoriteMottoStorage = mottoFavoriteStorageMock.Object;

        var rootNavigationServiceMock = new Mock<IRootNavigationService>();
        var mockRootNavigationService = rootNavigationServiceMock.Object;

        var mainWindowViewModel = new MainWindowViewModel(
            mockWordStorage, mockRootNavigationService, mockFavoriteStorage, mockMistakeStorage, mockFavoriteMottoStorage);

        mainWindowViewModel.OnInitialized();
        wordStorageMock.Verify(p => p.IsInitialized, Times.Once);
        wordFavoriteStorageMock.Verify(p => p.IsInitialized, Times.Never);
        rootNavigationServiceMock.Verify(
            p => p.NavigateTo(RootNavigationConstant.InitializationView),
            Times.Once);
    }
    
    [Fact]
    public async Task OnInitializedAsync_Initialized() {
        var wordStorageMock = new Mock<IWordStorage>();
        wordStorageMock.Setup(p => p.IsInitialized).Returns(true);
        var mockWordStorage = wordStorageMock.Object;

        var favoriteStorageMock = new Mock<IWordFavoriteStorage>();
        favoriteStorageMock.Setup(p => p.IsInitialized).Returns(true);
        var mockFavoriteStorage = favoriteStorageMock.Object;
        
        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        mistakeStorageMock.Setup(p => p.IsInitialized).Returns(true);
        var mockMistakeStorage = mistakeStorageMock.Object;
        
        var mottoFavoriteStorageMock = new Mock<IMottoFavoriteStorage>();
        mottoFavoriteStorageMock.Setup(p => p.IsInitialized).Returns(true);
        var mockFavoriteMottoStorage = mottoFavoriteStorageMock.Object;

        var rootNavigationServiceMock = new Mock<IRootNavigationService>();
        var mockRootNavigationService = rootNavigationServiceMock.Object;

        var mainWindowViewModel = new MainWindowViewModel(
            mockWordStorage, mockRootNavigationService, mockFavoriteStorage, mockMistakeStorage, mockFavoriteMottoStorage);

        mainWindowViewModel.OnInitialized();
        wordStorageMock.Verify(p => p.IsInitialized, Times.Once);
        favoriteStorageMock.Verify(p => p.IsInitialized, Times.Once);
        rootNavigationServiceMock.Verify(
            p => p.NavigateTo(RootNavigationConstant.MainView), Times.Once);
    }
}