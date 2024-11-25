using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;
using Moq;

namespace DailyWordA.UnitTest.ViewModels;

public class TodayMottoViewModelTest {
    [Fact]
    public async Task OnInitialized_Default() {
        var todayImageToReturn = new TodayImage();
        
        var todayImageServiceMock = new Mock<ITodayImageService>();
        todayImageServiceMock.Setup(p => p.GetRandomImageAsync())
            .ReturnsAsync(todayImageToReturn);
        var mockImageService = todayImageServiceMock.Object;

        var todayMottoToReturn = new DailyMotto();
        var dailyMottoServiceMock = new Mock<IDailyMottoService>();
        dailyMottoServiceMock.Setup(p => p.GetTodayMottoAsync())
            .ReturnsAsync(todayMottoToReturn);
        var mockMottoService = dailyMottoServiceMock.Object;
        
        // 注意构造函数中已经自动调用了Update方法
        var todayMottoViewModel = new TodayMottoViewModel(mockMottoService, mockImageService, null, null);
        
        var todayImageList = new List<TodayImage>();
        var todayMottoList = new List<DailyMotto>();
        todayMottoViewModel.PropertyChanged += (sender, args) => {
            switch (args.PropertyName) {
                case nameof(TodayMottoViewModel.TodayImage):
                    todayImageList.Add(todayMottoViewModel.TodayImage);
                    break;
                case nameof(TodayMottoViewModel.TodayMotto):
                    todayMottoList.Add(todayMottoViewModel.TodayMotto);
                    break;
            }
        };
        
        while (todayImageList.Count != 1 || todayMottoList.Count != 1) {
            await Task.Delay(100);
        }
        
        Assert.Same(todayImageToReturn, todayImageList[0]);
        Assert.Same(todayMottoToReturn, todayMottoViewModel.TodayMotto);

        todayImageServiceMock.Verify(p => p.GetRandomImageAsync(), Times.Once);
        dailyMottoServiceMock.Verify(p => p.GetTodayMottoAsync(), Times.Once);
    }
    
    [Fact]
    public async Task ShowDetailCommandFunction_Default() {
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;

        var todayMottoViewModel = new TodayMottoViewModel(null, null,
            mockContentNavigationService, null);
        todayMottoViewModel.ShowDetail();
        contentNavigationServiceMock.Verify(
            p => p.NavigateTo(ContentNavigationConstant.MottoDetailView, null),
            Times.Once);
    }
}