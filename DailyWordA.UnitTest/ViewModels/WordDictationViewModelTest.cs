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
        var mockMistakeStorage = mistakeStorageMock.Object;
        
        var wordDictationViewModel = new WordDictationViewModel(mockWordStorage, mockContentNavigationService, null, mockMistakeStorage, null);
        await Task.Delay(1000);
        
        var oldWord = wordDictationViewModel.CorrectWord;
        Assert.NotNull(oldWord);
        Assert.Equal(oldWord, wordToReturn);
        
        // wordDictationViewModel.Update();
        // await Task.Delay(1000);
        // Assert.NotEqual(oldWord, wordDictationViewModel.CorrectWord);
        // await wordStorage.CloseAsync();
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
        
        wordDictationViewModel.InputWord = wordDictationViewModel.CorrectWord.Word;
        await wordDictationViewModel.CommitAsync();
        
        Assert.True(wordDictationViewModel.HasAnswered);
        Assert.Equal("恭喜您回答正确！", wordDictationViewModel.ResultText);
        // await wordStorage.CloseAsync();
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
        // await wordStorage.CloseAsync();
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