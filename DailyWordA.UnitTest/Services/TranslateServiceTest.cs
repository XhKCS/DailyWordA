using DailyWordA.Library.Services;
using Moq;

namespace DailyWordA.UnitTest.Services;

public class TranslateServiceTest {
    [Fact]
    public async Task Translate_ReturnIsNotNullOrWhiteSpace() {
        var alertServiceMock = new Mock<IAlertService>();
        var mockAlertService = alertServiceMock.Object;
        var translateService = new TranslateService(mockAlertService);
        var sourceText = "Hello,World";
        var result = await translateService.Translate(sourceText, "auto", "zh");
        Assert.False(string.IsNullOrWhiteSpace(result));
        Assert.Equal("你好，世界", result);
        alertServiceMock.Verify(
            p => p.AlertAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }
    
}
