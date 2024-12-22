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
        var wordMistakeList = new List<WordMistake>() {new WordMistake {WordId = 1, IsInNote = true, Timestamp = DateTime.Now}};
        mistakeStorageMock.Setup(p => p.GetMistakeListAsync()).ReturnsAsync(wordMistakeList);
        var mockMistakeStorage = mistakeStorageMock.Object;

        
        var wordleGameViewModel = new WordleGameViewModel(mockWordStorage, mockContentNavigationService, mockMistakeStorage, null);
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
        
        wordleGameViewModel.IsFromMistake = true;
        wordleGameViewModel.Update();
        await Task.Delay(1000);
        mistakeStorageMock.Verify(p => p.GetMistakeListAsync(), Times.Once);
        
        // await wordStorage.CloseAsync();
    }
    
    [Fact]
    public async Task Update_MistakeListEmpty() {
        // var wordStorage = await WordStorageHelper.GetInitializedWordStorage();
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
        
        var wordleGameViewModel = new WordleGameViewModel(mockWordStorage, mockContentNavigationService, mockMistakeStorage, null);
        await Task.Delay(1000);
        
        wordleGameViewModel.IsFromMistake = true;
        wordleGameViewModel.Update();
        await Task.Delay(1000);
        mistakeStorageMock.Verify(p => p.GetMistakeListAsync(), Times.Once);
        
        Assert.False(wordleGameViewModel.IsFromMistake);
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
        
        var alertServiceMock = new Mock<IAlertService>();
        var mockAlertService = alertServiceMock.Object;
        
        var wordleGameViewModel = new WordleGameViewModel(mockWordStorage, null, mockMistakeStorage, mockAlertService);
        await Task.Delay(1000);
        
        wordleGameViewModel.IsFromMistake = true;
        await wordleGameViewModel.ChangeSource();
        
        Assert.False(wordleGameViewModel.IsFromMistake);
        alertServiceMock.Verify(p => p.AlertAsync("当前错题本为空", "当前错题本还没有单词哦，已自动切换回默认来源~"), Times.Once);
    }
    
    [Fact]
    public async Task ChangeSource_ToFromMistake_NotEmpty() {
        // var wordStorage = await WordStorageHelper.GetInitializedWordStorage();
        var wordStorageMock = new Mock<IWordStorage>();
        var wordToReturn = new WordObject { Word = "apple"};
        wordStorageMock.Setup(p => p.GetRandomWordAsync()).ReturnsAsync(wordToReturn);
        var mockWordStorage = wordStorageMock.Object;
        
        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        var wordMistakeList = new List<WordMistake>() {new WordMistake {WordId = 1, IsInNote = true, Timestamp = DateTime.Now}};
        mistakeStorageMock.Setup(p => p.GetMistakeListAsync()).ReturnsAsync(wordMistakeList);
        var mockMistakeStorage = mistakeStorageMock.Object;
        
        var alertServiceMock = new Mock<IAlertService>();
        var mockAlertService = alertServiceMock.Object;
        
        var wordleGameViewModel = new WordleGameViewModel(mockWordStorage, null, mockMistakeStorage, mockAlertService);
        await Task.Delay(1000);
        
        wordleGameViewModel.IsFromMistake = true;
        await wordleGameViewModel.ChangeSource();
        
        Assert.True(wordleGameViewModel.IsFromMistake);
        alertServiceMock.Verify(p => p.AlertAsync("切换成功", "测验单词来源已切换为：仅来自错题本"), Times.Once);
    }
    
    [Fact]
    public async Task ChangeSource_ToNotFromMistake() {
        // var wordStorage = await WordStorageHelper.GetInitializedWordStorage();
        var wordStorageMock = new Mock<IWordStorage>();
        var wordToReturn = new WordObject { Word = "apple"};
        wordStorageMock.Setup(p => p.GetRandomWordAsync()).ReturnsAsync(wordToReturn);
        var mockWordStorage = wordStorageMock.Object;

        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        var mockMistakeStorage = mistakeStorageMock.Object;
        
        var alertServiceMock = new Mock<IAlertService>();
        var mockAlertService = alertServiceMock.Object;
        
        var wordleGameViewModel = new WordleGameViewModel(mockWordStorage, null, mockMistakeStorage, mockAlertService);
        await Task.Delay(1000);
        // var oldWord = wordleGameViewModel.CorrectWord;
        
        wordleGameViewModel.IsFromMistake = false;
        await wordleGameViewModel.ChangeSource();
        
        Assert.False(wordleGameViewModel.IsFromMistake);
        await Task.Delay(1000);
        wordStorageMock.Verify(p => p.GetRandomWordAsync(), Times.Exactly(2));
        alertServiceMock.Verify(p => p.AlertAsync("切换成功", "测验单词来源已切换为：默认来源（从所有单词中随机抽取）"), Times.Once);
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
        
        var wordleGameViewModel = new WordleGameViewModel(mockWordStorage, mockContentNavigationService, mockMistakeStorage, null);
        await Task.Delay(1000);

        wordleGameViewModel.IsFromMistake = true;
        wordleGameViewModel.InputWord = wordleGameViewModel.CorrectWord.Word;
        await wordleGameViewModel.CommitAsync();
        
        Assert.True(wordleGameViewModel.HasFinished);
        Assert.Equal($"恭喜您在{wordleGameViewModel.CurrentAttemptRow}次尝试后回答正确！", wordleGameViewModel.ResultText);
        mistakeStorageMock.Verify(p => p.SaveMistakeAsync(new WordMistake {
            WordId = wordToReturn.Id,
            IsInNote = false
        }), Times.Once);
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
        
        var wordleGameViewModel = new WordleGameViewModel(mockWordStorage, mockContentNavigationService, mockMistakeStorage, null);
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
        
        var wordleGameViewModel = new WordleGameViewModel(mockWordStorage, mockContentNavigationService, mockMistakeStorage, null);
        await Task.Delay(1000);

        wordleGameViewModel.CurrentAttemptRow = 5;
        wordleGameViewModel.InputWord = "***";
        await wordleGameViewModel.CommitAsync();
        
        Assert.True(wordleGameViewModel.HasFinished);
        Assert.Equal($"正确答案是{wordleGameViewModel.CorrectWord.Word}  很遗憾，您没有回答正确哦~", wordleGameViewModel.ResultText);
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
        
        var wordleGameViewModel = new WordleGameViewModel(mockWordStorage, mockContentNavigationService, mockMistakeStorage, null);
        await Task.Delay(1000);
        
        wordleGameViewModel.ShowDetail();
        contentNavigationServiceMock.Verify(
            p => p.NavigateTo(
                ContentNavigationConstant.WordDetailView, wordleGameViewModel.CorrectWord),
            Times.Once);
        
    }
}