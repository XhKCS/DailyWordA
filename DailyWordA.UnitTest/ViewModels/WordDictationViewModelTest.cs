using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;
using DailyWordA.UnitTest.Helpers;
using Moq;

namespace DailyWordA.UnitTest.ViewModels;

public class WordDictationViewModelTest {
    [Fact]
    public async Task Update_Default() {
        // var wordStorage = await WordStorageHelper.GetInitializedWordStorage();
        var wordStorageMock = new Mock<IWordStorage>();
        var wordToReturn = new WordObject { Word = "apple"};
        wordStorageMock.Setup(p => p.GetRandomWordAsync()).ReturnsAsync(wordToReturn);
        var mockWordStorage = wordStorageMock.Object;
        
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;

        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        var wordMistakeList = new List<WordMistake>() {new WordMistake {WordId = 1, IsInNote = true, Timestamp = DateTime.Now}};
        mistakeStorageMock.Setup(p => p.GetMistakeListAsync()).ReturnsAsync(wordMistakeList);
        var mockMistakeStorage = mistakeStorageMock.Object;
        
        var wordDictationViewModel = new WordDictationViewModel(mockWordStorage, mockContentNavigationService, null, mockMistakeStorage, null);
        await Task.Delay(1000);
        
        var oldWord = wordDictationViewModel.CorrectWord;
        Assert.NotNull(oldWord);
        Assert.Equal(oldWord, wordToReturn);

        wordDictationViewModel.IsFromMistake = true;
        wordDictationViewModel.Update();
        await Task.Delay(1000);
        mistakeStorageMock.Verify(p => p.GetMistakeListAsync(), Times.Once);

        // wordDictationViewModel.Update();
        // await wordStorage.CloseAsync();
    }
    
    [Fact]
    public async Task Update_MistakeListEmpty() {
        var wordStorageMock = new Mock<IWordStorage>();
        var wordToReturn = new WordObject { Word = "apple"};
        wordStorageMock.Setup(p => p.GetRandomWordAsync()).ReturnsAsync(wordToReturn);
        var mockWordStorage = wordStorageMock.Object;
        
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;

        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        var emptyList = new List<WordMistake>();
        mistakeStorageMock.Setup(p => p.GetMistakeListAsync()).ReturnsAsync(emptyList);
        var mockMistakeStorage = mistakeStorageMock.Object;
        
        var wordDictationViewModel = new WordDictationViewModel(mockWordStorage, mockContentNavigationService, null, mockMistakeStorage, null);
        await Task.Delay(1000);
        
        wordDictationViewModel.IsFromMistake = true;
        wordDictationViewModel.Update();
        await Task.Delay(1000);
        mistakeStorageMock.Verify(p => p.GetMistakeListAsync(), Times.Once);
        
        Assert.False(wordDictationViewModel.IsFromMistake);
        wordStorageMock.Verify(p => p.GetRandomWordAsync(), Times.Exactly(2));
        
        // await wordStorage.CloseAsync();
    }
    
    [Fact]
    public async Task ChangeSource_ToFromMistake_Empty() {
        var wordStorageMock = new Mock<IWordStorage>();
        var mockWordStorage = wordStorageMock.Object;

        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        var emptyList = new List<WordMistake>();
        mistakeStorageMock.Setup(p => p.GetMistakeListAsync()).ReturnsAsync(emptyList);
        var mockMistakeStorage = mistakeStorageMock.Object;
        
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;
        
        var alertServiceMock = new Mock<IAlertService>();
        var mockAlertService = alertServiceMock.Object;
        
        var wordDictationViewModel = new WordDictationViewModel(mockWordStorage, mockContentNavigationService, null, mockMistakeStorage, mockAlertService);
        await Task.Delay(1000);
        
        wordDictationViewModel.IsFromMistake = true;
        await wordDictationViewModel.ChangeSource();
        
        Assert.False(wordDictationViewModel.IsFromMistake);
        alertServiceMock.Verify(p => p.AlertAsync("当前错题本为空", "当前错题本还没有单词哦，已自动切换回默认来源~"), Times.Once);
    }
    
    [Fact]
    public async Task ChangeSource_ToFromMistake_NotEmpty() {
        var wordStorageMock = new Mock<IWordStorage>();
        var mockWordStorage = wordStorageMock.Object;

        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        var wordMistakeList = new List<WordMistake>() {new WordMistake {WordId = 1, IsInNote = true, Timestamp = DateTime.Now}};
        mistakeStorageMock.Setup(p => p.GetMistakeListAsync()).ReturnsAsync(wordMistakeList);
        var mockMistakeStorage = mistakeStorageMock.Object;
        
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;
        
        var alertServiceMock = new Mock<IAlertService>();
        var mockAlertService = alertServiceMock.Object;
        
        var wordDictationViewModel = new WordDictationViewModel(mockWordStorage, mockContentNavigationService, null, mockMistakeStorage, mockAlertService);
        await Task.Delay(1000);
        
        wordDictationViewModel.IsFromMistake = true;
        await wordDictationViewModel.ChangeSource();
        
        Assert.True(wordDictationViewModel.IsFromMistake);
        alertServiceMock.Verify(p => p.AlertAsync("切换成功", "测验单词来源已切换为：仅来自错题本"), Times.Once);
    }
    
    [Fact]
    public async Task ChangeSource_ToNotFromMistake() {
        var wordStorageMock = new Mock<IWordStorage>();
        var mockWordStorage = wordStorageMock.Object;

        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        var wordMistakeList = new List<WordMistake>() {new WordMistake {WordId = 1, IsInNote = true, Timestamp = DateTime.Now}};
        mistakeStorageMock.Setup(p => p.GetMistakeListAsync()).ReturnsAsync(wordMistakeList);
        var mockMistakeStorage = mistakeStorageMock.Object;
        
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;
        
        var alertServiceMock = new Mock<IAlertService>();
        var mockAlertService = alertServiceMock.Object;
        
        var wordDictationViewModel = new WordDictationViewModel(mockWordStorage, mockContentNavigationService, null, mockMistakeStorage, mockAlertService);
        await Task.Delay(1000);
        
        wordDictationViewModel.IsFromMistake = false;
        await wordDictationViewModel.ChangeSource();
        
        Assert.False(wordDictationViewModel.IsFromMistake);
        await Task.Delay(1000);
        wordStorageMock.Verify(p => p.GetRandomWordAsync(), Times.Exactly(2));
        alertServiceMock.Verify(p => p.AlertAsync("切换成功", "测验单词来源已切换为：默认来源（从所有单词中随机抽取）"), Times.Once);
    }
    
    [Fact]
    public async Task Commit_Correct()
    {
        var wordStorageMock = new Mock<IWordStorage>();
        var wordToReturn = new WordObject { Word = "apple"};
        wordStorageMock.Setup(p => p.GetRandomWordAsync()).ReturnsAsync(wordToReturn);
        var mockWordStorage = wordStorageMock.Object;
        
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;
        
        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        var mockMistakeStorage = mistakeStorageMock.Object;
        
        var wordDictationViewModel = new WordDictationViewModel(mockWordStorage, mockContentNavigationService, null, mockMistakeStorage, null);
        await Task.Delay(1000);
        
        wordDictationViewModel.IsFromMistake = true;
        wordDictationViewModel.InputWord = wordDictationViewModel.CorrectWord.Word;
        await wordDictationViewModel.CommitAsync();
        
        Assert.True(wordDictationViewModel.HasAnswered);
        Assert.Equal("恭喜您回答正确！", wordDictationViewModel.ResultText);
        mistakeStorageMock.Verify(p => p.SaveMistakeAsync(new WordMistake {
            WordId = wordToReturn.Id,
            IsInNote = false
        }), Times.Once);
    }
    
    [Fact]
    public async Task Commit_Wrong() {
        var wordStorageMock = new Mock<IWordStorage>();
        var wordToReturn = new WordObject { Word = "apple"};
        wordStorageMock.Setup(p => p.GetRandomWordAsync()).ReturnsAsync(wordToReturn);
        var mockWordStorage = wordStorageMock.Object;
        
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;
        
        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        var mockMistakeStorage = mistakeStorageMock.Object;
        
        var wordDictationViewModel = new WordDictationViewModel(mockWordStorage, mockContentNavigationService, null, mockMistakeStorage, null);
        await Task.Delay(1000);
        
        wordDictationViewModel.InputWord = "***";
        await wordDictationViewModel.CommitAsync();
        
        Assert.True(wordDictationViewModel.HasAnswered);
        Assert.Equal("很遗憾，回答错误啦~", wordDictationViewModel.ResultText);
        mistakeStorageMock.Verify(p => p.SaveMistakeAsync(new WordMistake {
            WordId = wordToReturn.Id,
            IsInNote = true,
        }), Times.Once);
    }
    
    [Fact]
    public async Task ShowDetailCommandFunction_Default() {
        var wordStorageMock = new Mock<IWordStorage>();
        var wordToReturn = new WordObject { Word = "apple"};
        wordStorageMock.Setup(p => p.GetRandomWordAsync()).ReturnsAsync(wordToReturn);
        var mockWordStorage = wordStorageMock.Object;
        
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;
        
        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        var mockMistakeStorage = mistakeStorageMock.Object;
        
        var wordDictationViewModel = new WordDictationViewModel(mockWordStorage, mockContentNavigationService, null, mockMistakeStorage, null);
        await Task.Delay(1000);
        
        wordDictationViewModel.ShowDetail();
        contentNavigationServiceMock.Verify(
            p => p.NavigateTo(
                ContentNavigationConstant.WordDetailView, wordDictationViewModel.CorrectWord),
            Times.Once);
    }
}