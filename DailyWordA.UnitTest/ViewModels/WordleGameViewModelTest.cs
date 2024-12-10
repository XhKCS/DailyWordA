using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;
using DailyWordA.UnitTest.Helpers;
using Moq;

namespace DailyWordA.UnitTest.ViewModels;

public class WordleGameViewModelTest {
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
        
        var wordleGameViewModel = new WordleGameViewModel(mockWordStorage, mockContentNavigationService, mockMistakeStorage);
        await Task.Delay(1000);
        
        Assert.False(wordleGameViewModel.HasFinished);
        var oldWord = wordleGameViewModel.CorrectWord;
        Assert.NotNull(oldWord);
        Assert.True(wordleGameViewModel.CorrectWord.Word.Length <= 5);
        Assert.Equal(oldWord, wordToReturn);
        
        Assert.Equal(5, wordleGameViewModel.GridLetters[0].Count);
        var letterStatus = wordleGameViewModel.GridLetters[0][0];
        Assert.Equal(" ", letterStatus.Letter);
        Assert.Equal("White", letterStatus.Background);
        
        Assert.Equal("LLLLL", wordleGameViewModel.Mask);
        Assert.Equal(0, wordleGameViewModel.CurrentAttemptRow);
        Assert.Equal("请输入第1次猜的单词：", wordleGameViewModel.HintText);
        
        // Assert.NotEqual(oldWord, wordleGameViewModel.CorrectWord);
        // await wordStorage.CloseAsync();
    }
    
    [Fact]
    public async Task Commit_Correct() {
        var wordStorageMock = new Mock<IWordStorage>();
        var wordToReturn = new WordObject { Word = "apple"};
        wordStorageMock.Setup(p => p.GetRandomWordAsync()).ReturnsAsync(wordToReturn);
        var mockWordStorage = wordStorageMock.Object;
        
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;

        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        var mockMistakeStorage = mistakeStorageMock.Object;
        
        var wordleGameViewModel = new WordleGameViewModel(mockWordStorage, mockContentNavigationService, mockMistakeStorage);
        await Task.Delay(1000);
        
        wordleGameViewModel.InputWord = wordleGameViewModel.CorrectWord.Word;
        await wordleGameViewModel.CommitAsync();
        
        Assert.True(wordleGameViewModel.HasFinished);
        Assert.Equal($"恭喜您在{wordleGameViewModel.CurrentAttemptRow}次尝试后回答正确！", wordleGameViewModel.ResultText);
        
    }
    
    [Fact]
    public async Task Commit_Wrong_LessThanMaxTimes() {
        var wordStorageMock = new Mock<IWordStorage>();
        var wordToReturn = new WordObject { Word = "apple"};
        wordStorageMock.Setup(p => p.GetRandomWordAsync()).ReturnsAsync(wordToReturn);
        var mockWordStorage = wordStorageMock.Object;
        
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;

        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        var mockMistakeStorage = mistakeStorageMock.Object;
        
        var wordleGameViewModel = new WordleGameViewModel(mockWordStorage, mockContentNavigationService, mockMistakeStorage);
        await Task.Delay(1000);
        
        wordleGameViewModel.InputWord = "***";
        await wordleGameViewModel.CommitAsync();
        
        Assert.False(wordleGameViewModel.HasFinished);
        Assert.Equal("不对哦，再试试吧~", wordleGameViewModel.ResultText);
        Assert.Equal($"请输入第{wordleGameViewModel.CurrentAttemptRow+1}次猜的单词：", wordleGameViewModel.HintText);
        
    }
    
    [Fact]
    public async Task Commit_Wrong_ReachMaxTimes() {
        var wordStorageMock = new Mock<IWordStorage>();
        var wordToReturn = new WordObject { Word = "apple"};
        wordStorageMock.Setup(p => p.GetRandomWordAsync()).ReturnsAsync(wordToReturn);
        var mockWordStorage = wordStorageMock.Object;
        
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;

        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        var mockMistakeStorage = mistakeStorageMock.Object;
        
        var wordleGameViewModel = new WordleGameViewModel(mockWordStorage, mockContentNavigationService, mockMistakeStorage);
        await Task.Delay(1000);

        wordleGameViewModel.CurrentAttemptRow = 5;
        wordleGameViewModel.InputWord = "***";
        await wordleGameViewModel.CommitAsync();
        
        Assert.True(wordleGameViewModel.HasFinished);
        Assert.Equal($"正确答案是{wordleGameViewModel.CorrectWord.Word}  很遗憾，您没有回答正确哦~", wordleGameViewModel.ResultText);
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
        
        var wordleGameViewModel = new WordleGameViewModel(mockWordStorage, mockContentNavigationService, mockMistakeStorage);
        await Task.Delay(1000);
        
        wordleGameViewModel.ShowDetail();
        contentNavigationServiceMock.Verify(
            p => p.NavigateTo(
                ContentNavigationConstant.WordDetailView, wordleGameViewModel.CorrectWord),
            Times.Once);
        
    }
}