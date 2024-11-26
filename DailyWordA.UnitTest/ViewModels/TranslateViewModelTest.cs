using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;
using Moq;

namespace DailyWordA.UnitTest.ViewModels;

public class TranslateViewModelTest {
    [Fact]
    public async Task TranslateAsync_Default() {
        // var translateServiceMock = new Mock<ITranslateService>();
        // var mockTranslateService = translateServiceMock.Object;
        var alertServiceMock = new Mock<IAlertService>();
        var mockAlertService = alertServiceMock.Object;
        var translateService = new TranslateService(mockAlertService);
        var translateViewModel = new TranslateViewModel(translateService);
       
        translateViewModel.SourceText = "Hello,World";
        translateViewModel.LanguageType = TargetLanguageType.ToChineseType;
        await translateViewModel.TranslateAsync();
        Assert.Equal("你好，世界", translateViewModel.TargetText);
       
        translateViewModel.SourceText = "行胜于言。";
        translateViewModel.LanguageType = TargetLanguageType.ToEnglishType;
        await translateViewModel.TranslateAsync();
        Assert.Equal("Actions speak louder than words.", translateViewModel.TargetText);
    }
}
