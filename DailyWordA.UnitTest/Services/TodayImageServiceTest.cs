using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using Moq;

namespace DailyWordA.UnitTest.Services;

public class TodayImageServiceTest {
    [Fact(Skip = "依赖远程服务的测试")]
    public async Task CheckUpdateAsync_TodayImageNotExpired() {
        var todayImageToReturn = new TodayImage {
            StartDate = "19700101",
            ExpiresAt = DateTime.Now + TimeSpan.FromHours(1), //设置当前未过期
            Copyright = "Copyright",
            Title = "Today's image",
        };
        
        var todayImageStorageMock = new Mock<ITodayImageStorage>();
        todayImageStorageMock.Setup(p => p.GetTodayImageAsync(false))
            .ReturnsAsync(todayImageToReturn);
        var mockTodayImageStorage = todayImageStorageMock.Object;

        var alertServiceMock = new Mock<IAlertService>();
        var mockAlertService = alertServiceMock.Object;

        var todayImageService =
            new TodayImageService( mockAlertService, mockTodayImageStorage);
        var checkUpdateResult = await todayImageService.CheckUpdateAsync();

        Assert.False(checkUpdateResult.HasUpdate);
        todayImageStorageMock.Verify(p => p.GetTodayImageAsync(false),
            Times.Once);
        todayImageStorageMock.Verify(
            p => p.SaveTodayImageAsync(It.IsAny<TodayImage>(),
                It.IsAny<bool>()), Times.Never);
        alertServiceMock.Verify(
            p => p.AlertAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }
    
    [Fact(Skip = "依赖远程服务的测试")]
    public async Task CheckUpdateAsync_TodayImageExpired() {
        var todayImageToReturn = new TodayImage {
            StartDate = "19700101",
            ExpiresAt = DateTime.Now - TimeSpan.FromHours(1), //设置当前已过期
            Copyright = "Copyright",
            Title = "Today's image",
        };
        
        var todayImageStorageMock = new Mock<ITodayImageStorage>();
        todayImageStorageMock.Setup(p => p.GetTodayImageAsync(false))
            .ReturnsAsync(todayImageToReturn);
        var mockTodayImageStorage = todayImageStorageMock.Object;

        var alertServiceMock = new Mock<IAlertService>();
        var mockAlertService = alertServiceMock.Object;

        var bingImageService =
            new TodayImageService(mockAlertService, mockTodayImageStorage);
        var checkUpdateResult = await bingImageService.CheckUpdateAsync();

        Assert.True(checkUpdateResult.HasUpdate);
        todayImageStorageMock.Verify(p => p.GetTodayImageAsync(false),
            Times.Once);
        todayImageStorageMock.Verify(
            p => p.SaveTodayImageAsync(checkUpdateResult.TodayImage, false),
            Times.Once);
        alertServiceMock.Verify(
            p => p.AlertAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }
}