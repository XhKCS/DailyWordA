using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;
using DailyWordA.UnitTest.Helpers;
using Moq;

namespace DailyWordA.UnitTest.ViewModels;

public class WordleGameViewModelTest {
    [Fact]
    public async Task Update_Default() {
        var wordStorage = await WordStorageHelper.GetInitializedWordStorage();
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;

        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        var mockMistakeStorage = mistakeStorageMock.Object;
        
        var wordleGameViewModel = new WordleGameViewModel(wordStorage, mockContentNavigationService, mockMistakeStorage);
        await Task.Delay(1000);
        
        Assert.False(wordleGameViewModel.HasFinished);
        var oldWord = wordleGameViewModel.CorrectWord;
        Assert.NotNull(oldWord);
        
        wordleGameViewModel.Update();
        await Task.Delay(1000);
        
        Assert.NotEqual(oldWord, wordleGameViewModel.CorrectWord);
        await wordStorage.CloseAsync();
    }
    
    [Fact]
    public async Task Commit_Correct() {
        var wordStorage = await WordStorageHelper.GetInitializedWordStorage();
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;

        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        var mockMistakeStorage = mistakeStorageMock.Object;
        
        var wordleGameViewModel = new WordleGameViewModel(wordStorage, mockContentNavigationService, mockMistakeStorage);
        await Task.Delay(1000);
        
        wordleGameViewModel.InputWord = wordleGameViewModel.CorrectWord.Word;
        await wordleGameViewModel.CommitAsync();
        
        Assert.True(wordleGameViewModel.HasFinished);
        Assert.Equal($"恭喜您在{wordleGameViewModel.CurrentAttemptRow}次尝试后回答正确！", wordleGameViewModel.ResultText);
        await wordStorage.CloseAsync();
    }
    
    [Fact]
    public async Task Commit_Wrong() {
        var wordStorage = await WordStorageHelper.GetInitializedWordStorage();
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;

        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        var mockMistakeStorage = mistakeStorageMock.Object;
        
        var wordleGameViewModel = new WordleGameViewModel(wordStorage, mockContentNavigationService, mockMistakeStorage);
        await Task.Delay(1000);
        
        wordleGameViewModel.InputWord = "***";
        await wordleGameViewModel.CommitAsync();
        
        Assert.False(wordleGameViewModel.HasFinished);
        Assert.Equal("不对哦，再试试吧~", wordleGameViewModel.ResultText);
        Assert.Equal($"请输入第{wordleGameViewModel.CurrentAttemptRow+1}次猜的单词：", wordleGameViewModel.HintText);
        await wordStorage.CloseAsync();
    }
    
    [Fact]
    public async Task ShowDetailCommandFunction_Default() {
        var wordStorage = await WordStorageHelper.GetInitializedWordStorage();
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;

        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        var mockMistakeStorage = mistakeStorageMock.Object;
        
        var wordleGameViewModel = new WordleGameViewModel(wordStorage, mockContentNavigationService, mockMistakeStorage);
        await Task.Delay(1000);
        
        wordleGameViewModel.ShowDetail();
        contentNavigationServiceMock.Verify(
            p => p.NavigateTo(
                ContentNavigationConstant.WordDetailView, wordleGameViewModel.CorrectWord),
            Times.Once);
        await wordStorage.CloseAsync();
    }
}