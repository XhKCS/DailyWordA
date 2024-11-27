using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;
using Moq;

namespace DailyWordA.UnitTest.ViewModels;

public class MainViewModelTest {
    [Fact]
    public void OpenPane_Default() {
        var mainViewModel = new MainViewModel(null);
        Assert.False(mainViewModel.IsPaneOpen);
        mainViewModel.OpenPane();
        Assert.True(mainViewModel.IsPaneOpen);
    }
    
    [Fact]
    public void ClosePane_Default() {
        var mainViewModel = new MainViewModel(null);
        mainViewModel.OpenPane();
        Assert.True(mainViewModel.IsPaneOpen);
        mainViewModel.ClosePane();
        Assert.False(mainViewModel.IsPaneOpen);
    }
    
    [Fact]
    public void PushContent_Default() {
        var mainViewModel = new MainViewModel(null);
        Assert.Null(mainViewModel.Content);
        Assert.Empty(mainViewModel.ContentStack);

        var content = new Mock<ViewModelBase>().Object;
        mainViewModel.PushContent(content);
        Assert.Same(content, mainViewModel.Content);
        Assert.Single(mainViewModel.ContentStack);
        Assert.Same(content, mainViewModel.ContentStack[0]);
    }

    [Fact]
    public void SetMenuAndContent_Default() {
        var mainViewModel = new MainViewModel(null);
        var menuItem = MenuItem.MenuItems.First(p =>
            p.View == MenuNavigationConstant.TodayWordView);
        var content = new Mock<ViewModelBase>().Object;
        mainViewModel.SetMenuAndContent(menuItem.View, content);

        Assert.Same(content, mainViewModel.Content);
        Assert.Single(mainViewModel.ContentStack);
        Assert.Same(content, mainViewModel.ContentStack[0]);
        Assert.Same(menuItem, mainViewModel.SelectedMenuItem);
        Assert.Equal(menuItem.Name, mainViewModel.Title);
        Assert.False(mainViewModel.IsPaneOpen);
    }
    
    [Fact]
    public void OnMenuTapped_SelectedMenuItemIsNull() {
        var menuNavigationServiceMock = new Mock<IMenuNavigationService>();
        var mockMenuNavigationService = menuNavigationServiceMock.Object;
        var mainViewModel = new MainViewModel(mockMenuNavigationService);
        mainViewModel.OnMenuTapped();
        
        menuNavigationServiceMock.Verify(p => 
                p.NavigateTo(It.IsAny<string>(), null), Times.Never);
    }
    
    [Fact]
    public void OnMenuTapped_SelectedMenuItemIsNotNull() {
        var menuNavigationServiceMock = new Mock<IMenuNavigationService>();
        var mockMenuNavigationService = menuNavigationServiceMock.Object;

        var menuItem = MenuItem.MenuItems.First(p =>
            p.View == MenuNavigationConstant.TodayWordView);
        var mainViewModel = new MainViewModel(mockMenuNavigationService);
        mainViewModel.SelectedMenuItem = menuItem;
        mainViewModel.OnMenuTapped();

        menuNavigationServiceMock.Verify(p => 
                p.NavigateTo(menuItem.View, null), Times.Once);
    }
    
    [Fact]
    public void GoBack_ContentStackCountIs0() {
        var mainViewModel = new MainViewModel(null);
        Assert.Empty(mainViewModel.ContentStack);
        mainViewModel.GoBack();
        Assert.Empty(mainViewModel.ContentStack);
    }

    [Fact]
    public void GoBack_ContentStackCountIs1() {
        var mainViewModel = new MainViewModel(null);
        var content = new Mock<ViewModelBase>().Object;

        mainViewModel.PushContent(content);
        mainViewModel.GoBack();
        Assert.Single(mainViewModel.ContentStack);
        Assert.Same(content, mainViewModel.ContentStack[0]);
    }

    [Fact]
    public void GoBack_ContentStackCountIs2() {
        var mainViewModel = new MainViewModel(null);
        var content1 = new Mock<ViewModelBase>().Object;
        var content2 = new Mock<ViewModelBase>().Object;

        mainViewModel.PushContent(content1);
        mainViewModel.PushContent(content2);
        mainViewModel.GoBack();
        Assert.Single(mainViewModel.ContentStack);
        Assert.Same(content1, mainViewModel.ContentStack[0]);
    }
}