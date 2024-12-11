using DailyWordA.Library.Services;
using Moq;
using Xunit.Abstractions;

namespace DailyWordA.UnitTest.Services;

public class DailyMottoServiceTest {
    private readonly ITestOutputHelper _testOutputHelper;

    public DailyMottoServiceTest(ITestOutputHelper testOutputHelper) {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task GetTodayMottoAsync_Default() {
        var alertServiceMock = new Mock<IAlertService>();
        var mockAlertService = alertServiceMock.Object;
        
        var dailyMottoService = new DailyMottoService(mockAlertService);
        var todayMotto = await dailyMottoService.GetTodayMottoAsync();
        _testOutputHelper.WriteLine(todayMotto.Content);
        
        Assert.False(string.IsNullOrWhiteSpace(todayMotto.Content));
        alertServiceMock.Verify(
            p => p.AlertAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }
}