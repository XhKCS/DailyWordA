using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;
using Moq;

namespace DailyWordA.UnitTest.ViewModels;

public class SentenceOrganizationViewModelTest {
    [Fact]
    public async Task Update_Default() {
        var dailyMottoServiceMock = new Mock<IDailyMottoService>();
        var dailyMottoToReturn = new DailyMotto {
            Content = "We are the hero of our own story.",
            Translation = "我们都是我们自己故事里的英雄。",
            Date = "2024-12-23"
        };
        dailyMottoServiceMock.Setup(p => p.GetTodayMottoAsync()).ReturnsAsync(dailyMottoToReturn);
        var mockDailyMottoService = dailyMottoServiceMock.Object;
        
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;
        
        var sentenceOrganizationViewModel = new SentenceOrganizationViewModel(mockDailyMottoService, mockContentNavigationService);
        await Task.Delay(1000);
        
        Assert.False(sentenceOrganizationViewModel.HasAnswered);
        Assert.NotNull(sentenceOrganizationViewModel.CorrectSentence);
        Assert.Equal(dailyMottoToReturn, sentenceOrganizationViewModel.CorrectSentence);
        Assert.NotEmpty(sentenceOrganizationViewModel.WordStatusGroup);
        dailyMottoServiceMock.Verify(p => p.GetTodayMottoAsync(), Times.Once);
    }

    [Fact]
    public async Task SelectWord_Default() {
        var dailyMottoServiceMock = new Mock<IDailyMottoService>();
        var dailyMottoToReturn = new DailyMotto {
            Content = "We are the hero of our own story.",
            Translation = "我们都是我们自己故事里的英雄。",
            Date = "2024-12-23"
        };
        dailyMottoServiceMock.Setup(p => p.GetTodayMottoAsync()).ReturnsAsync(dailyMottoToReturn);
        var mockDailyMottoService = dailyMottoServiceMock.Object;
        
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;
        
        var sentenceOrganizationViewModel = new SentenceOrganizationViewModel(mockDailyMottoService, mockContentNavigationService);
        await Task.Delay(1000);
        
        Assert.Equal(string.Empty, sentenceOrganizationViewModel.CurrentSentence);
        sentenceOrganizationViewModel.SelectWord(new Location {RowIndex = 0, ColumnIndex = 0});
        sentenceOrganizationViewModel.SelectWord(new Location {RowIndex = 0, ColumnIndex = 1});
        Assert.Equal(2, sentenceOrganizationViewModel.CurrentWords.Count);
        
        sentenceOrganizationViewModel.SelectWord(new Location {RowIndex = 0, ColumnIndex = 0});
        Assert.Equal(1, sentenceOrganizationViewModel.CurrentWords.Count);
        Assert.False(sentenceOrganizationViewModel.WordStatusGroup[0][0].IsSelected);
        Assert.DoesNotContain(sentenceOrganizationViewModel.WordStatusGroup[0][0].Word, sentenceOrganizationViewModel.CurrentWords);
    }

    [Fact]
    public async Task Commit_Wrong() {
        var dailyMottoServiceMock = new Mock<IDailyMottoService>();
        var dailyMottoToReturn = new DailyMotto {
            Content = "We are the hero of our own story.",
            Translation = "我们都是我们自己故事里的英雄。",
            Date = "2024-12-23"
        };
        dailyMottoServiceMock.Setup(p => p.GetTodayMottoAsync()).ReturnsAsync(dailyMottoToReturn);
        var mockDailyMottoService = dailyMottoServiceMock.Object;
        
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;
        
        var sentenceOrganizationViewModel = new SentenceOrganizationViewModel(mockDailyMottoService, mockContentNavigationService);
        await Task.Delay(1000);
        
        sentenceOrganizationViewModel.CurrentSentence = "We are our hero of own the story.";
        sentenceOrganizationViewModel.Commit();
        Assert.True(sentenceOrganizationViewModel.HasAnswered);
        Assert.Equal("很遗憾，回答错误啦~", sentenceOrganizationViewModel.ResultText);
    }
    
    [Fact]
    public async Task Commit_Correct() {
        var dailyMottoServiceMock = new Mock<IDailyMottoService>();
        var dailyMottoToReturn = new DailyMotto {
            Content = "We are the hero of our own story.",
            Translation = "我们都是我们自己故事里的英雄。",
            Date = "2024-12-23"
        };
        dailyMottoServiceMock.Setup(p => p.GetTodayMottoAsync()).ReturnsAsync(dailyMottoToReturn);
        var mockDailyMottoService = dailyMottoServiceMock.Object;
        
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;
        
        var sentenceOrganizationViewModel = new SentenceOrganizationViewModel(mockDailyMottoService, mockContentNavigationService);
        await Task.Delay(1000);
        
        sentenceOrganizationViewModel.CurrentSentence = dailyMottoToReturn.Content;
        sentenceOrganizationViewModel.Commit();
        Assert.True(sentenceOrganizationViewModel.HasAnswered);
        Assert.Equal("恭喜您回答正确！", sentenceOrganizationViewModel.ResultText);
    }

    [Fact]
    public async Task ShowMottoDetail_Default() {
        var dailyMottoServiceMock = new Mock<IDailyMottoService>();
        var dailyMottoToReturn = new DailyMotto {
            Content = "We are the hero of our own story.",
            Translation = "我们都是我们自己故事里的英雄。",
            Date = "2024-12-23"
        };
        dailyMottoServiceMock.Setup(p => p.GetTodayMottoAsync()).ReturnsAsync(dailyMottoToReturn);
        var mockDailyMottoService = dailyMottoServiceMock.Object;
        
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;
        
        var sentenceOrganizationViewModel = new SentenceOrganizationViewModel(mockDailyMottoService, mockContentNavigationService);
        await Task.Delay(1000);
        
        sentenceOrganizationViewModel.ShowMottoDetail();
        contentNavigationServiceMock.Verify(p => 
            p.NavigateTo(ContentNavigationConstant.MottoDetailView, sentenceOrganizationViewModel.CorrectSentence), 
            Times.Once);
        
    }
}