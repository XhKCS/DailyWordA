using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;
using DailyWordA.UnitTest.Helpers;
using Moq;

namespace DailyWordA.UnitTest.ViewModels;

public class WordSelectionViewModelTest {
    [Fact]
    public async Task Update_Default() {
        var wordStorage = await WordStorageHelper.GetInitializedWordStorage();
        // var wordStorageMock = new Mock<IWordStorage>();
        // var wordToReturn = new WordObject { Word = "apple"};
        // wordStorageMock.Setup(p => p.GetRandomWordAsync()).ReturnsAsync(wordToReturn);
        // var mockWordStorage = wordStorageMock.Object;
        
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;

        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        var wordMistakeList = new List<WordMistake>() {new WordMistake {WordId = 1, IsInNote = true, Timestamp = DateTime.Now}};
        mistakeStorageMock.Setup(p => p.GetMistakeListAsync()).ReturnsAsync(wordMistakeList);
        var mockMistakeStorage = mistakeStorageMock.Object;
        
        var wordQuizViewModel = new WordSelectionViewModel(wordStorage, mockContentNavigationService, mockMistakeStorage, null);
        await Task.Delay(1000);
        
        Assert.Equal(4, wordQuizViewModel.QuizOptions.Count);
        var oldWord = wordQuizViewModel.CorrectWord;
        Assert.NotNull(oldWord);
        Assert.Contains(oldWord, wordQuizViewModel.QuizOptions);
        
        wordQuizViewModel.IsFromMistake = true;
        wordQuizViewModel.Update();
        await Task.Delay(1000);
        mistakeStorageMock.Verify(p => p.GetMistakeListAsync(), Times.Once);
        
        await wordStorage.CloseAsync();
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
        
        var wordQuizViewModel = new WordSelectionViewModel(mockWordStorage, mockContentNavigationService, mockMistakeStorage, null);
        await Task.Delay(1000);
        
        wordQuizViewModel.IsFromMistake = true;
        wordQuizViewModel.Update();
        await Task.Delay(1000);
        mistakeStorageMock.Verify(p => p.GetMistakeListAsync(), Times.Once);
        
        Assert.False(wordQuizViewModel.IsFromMistake);
        wordStorageMock.Verify(p => p.GetRandomWordAsync(), Times.AtLeastOnce);
        
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
        
        var wordQuizViewModel = new WordSelectionViewModel(mockWordStorage, null, mockMistakeStorage, mockAlertService);
        await Task.Delay(1000);
        
        wordQuizViewModel.IsFromMistake = true;
        await wordQuizViewModel.ChangeSource();
        
        Assert.False(wordQuizViewModel.IsFromMistake);
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
        
        var wordQuizViewModel = new WordSelectionViewModel(mockWordStorage, null, mockMistakeStorage, mockAlertService);
        await Task.Delay(1000);
        
        wordQuizViewModel.IsFromMistake = true;
        await wordQuizViewModel.ChangeSource();
        
        Assert.True(wordQuizViewModel.IsFromMistake);
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
        
        var wordQuizViewModel = new WordSelectionViewModel(mockWordStorage, null, mockMistakeStorage, mockAlertService);
        await Task.Delay(1000);
        // var oldWord = wordQuizViewModel.CorrectWord;
        
        wordQuizViewModel.IsFromMistake = false;
        await wordQuizViewModel.ChangeSource();
        
        Assert.False(wordQuizViewModel.IsFromMistake);
        await Task.Delay(1000);
        wordStorageMock.Verify(p => p.GetRandomWordAsync(), Times.Exactly(2));
        alertServiceMock.Verify(p => p.AlertAsync("切换成功", "测验单词来源已切换为：默认来源（从所有单词中随机抽取）"), Times.Once);
    }

    [Fact]
    public async Task SelectMode_Default() {
        // var wordStorage = await WordStorageHelper.GetInitializedWordStorage();
        var wordStorageMock = new Mock<IWordStorage>();
        var wordToReturn = new WordObject { Word = "apple"};
        wordStorageMock.Setup(p => p.GetRandomWordAsync()).ReturnsAsync(wordToReturn);
        var mockWordStorage = wordStorageMock.Object;
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;
        
        var wordQuizViewModel = new WordSelectionViewModel(mockWordStorage, mockContentNavigationService, null, null);
        await Task.Delay(1000);
        var oldWord = wordQuizViewModel.CorrectWord;
        
        Assert.Equal("英文选义", wordQuizViewModel.SelectedMode);
        wordQuizViewModel.SelectMode("中文选词");
        await Task.Delay(1000);
        
        Assert.Equal("中文选词", wordQuizViewModel.SelectedMode);
        // Assert.NotEqual(oldWord, wordQuizViewModel.CorrectWord);
        // await wordStorage.CloseAsync();
    }

    [Fact]
    public async Task RadioChecked_Default() {
        // var wordStorage = await WordStorageHelper.GetInitializedWordStorage();
        var wordStorageMock = new Mock<IWordStorage>();
        var wordToReturn = new WordObject { Id = 1, Word = "apple" };
        var wordOptionsToReturn = new List<WordObject> { wordToReturn, new WordObject{Id = 2, Word = "pear"}, 
            new WordObject{Id = 3, Word = "orange"}, new WordObject{Id = 4, Word = "watermelon"}};
        wordStorageMock.Setup(p => p.GetRandomWordAsync()).ReturnsAsync(wordToReturn);
        wordStorageMock.Setup(p => p.GetWordQuizOptionsAsync(wordToReturn)).ReturnsAsync(wordOptionsToReturn);
        var mockWordStorage = wordStorageMock.Object;
        
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;
        
        var wordQuizViewModel = new WordSelectionViewModel(mockWordStorage, mockContentNavigationService, null, null);
        await Task.Delay(1000);
        
        var selectedWord = wordQuizViewModel.QuizOptions[new Random().Next(0, 4)];
        wordQuizViewModel.RadioChecked(selectedWord);
        
        Assert.True(wordQuizViewModel.HasSelected);
        Assert.Equal(selectedWord, wordQuizViewModel.SelectedOption);
        // await wordStorage.CloseAsync();
    }

    [Fact]
    public async Task Commit_Correct() {
        // var wordStorage = await WordStorageHelper.GetInitializedWordStorage();
        var wordStorageMock = new Mock<IWordStorage>();
        var wordToReturn = new WordObject { Id = 1, Word = "apple" };
        var wordOptionsToReturn = new List<WordObject> { wordToReturn, new WordObject{Id = 2, Word = "pear"}, 
            new WordObject{Id = 3, Word = "orange"}, new WordObject{Id = 4, Word = "watermelon"}};
        wordStorageMock.Setup(p => p.GetRandomWordAsync()).ReturnsAsync(wordToReturn);
        wordStorageMock.Setup(p => p.GetWordQuizOptionsAsync(wordToReturn)).ReturnsAsync(wordOptionsToReturn);
        var mockWordStorage = wordStorageMock.Object;
        
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;
        
        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        var mockMistakeStorage = mistakeStorageMock.Object;
        
        var wordQuizViewModel = new WordSelectionViewModel(mockWordStorage, mockContentNavigationService, mockMistakeStorage, null);
        await Task.Delay(1000);
        
        wordQuizViewModel.IsFromMistake = true;
        wordQuizViewModel.RadioChecked(wordQuizViewModel.CorrectWord);
        await wordQuizViewModel.CommitAsync();
        
        Assert.True(wordQuizViewModel.HasAnswered);
        Assert.Equal("恭喜您回答正确！", wordQuizViewModel.ResultText);
        mistakeStorageMock.Verify(p => p.SaveMistakeAsync(new WordMistake {
            WordId = wordToReturn.Id,
            IsInNote = false
        }), Times.Once);
    }
    
    [Fact]
    public async Task Commit_Wrong() {
        // var wordStorage = await WordStorageHelper.GetInitializedWordStorage();
        var wordStorageMock = new Mock<IWordStorage>();
        var wordToReturn = new WordObject { Id = 1, Word = "apple" };
        var wordOptionsToReturn = new List<WordObject> { wordToReturn, new WordObject{Id = 2, Word = "pear"}, 
            new WordObject{Id = 3, Word = "orange"}, new WordObject{Id = 4, Word = "watermelon"}};
        wordStorageMock.Setup(p => p.GetRandomWordAsync()).ReturnsAsync(wordToReturn);
        wordStorageMock.Setup(p => p.GetWordQuizOptionsAsync(wordToReturn)).ReturnsAsync(wordOptionsToReturn);
        var mockWordStorage = wordStorageMock.Object;
        
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;
        
        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        var mockMistakeStorage = mistakeStorageMock.Object;
        
        var wordQuizViewModel = new WordSelectionViewModel(mockWordStorage, mockContentNavigationService, mockMistakeStorage, null);
        await Task.Delay(1000);

        var index = wordQuizViewModel.QuizOptions.IndexOf(wordQuizViewModel.CorrectWord);
        var wrongWord = wordQuizViewModel.QuizOptions[(index + 1) % 4];
        wordQuizViewModel.RadioChecked(wrongWord);
        await wordQuizViewModel.CommitAsync();
        
        Assert.True(wordQuizViewModel.HasAnswered);
        Assert.Equal("很遗憾，回答错误啦~", wordQuizViewModel.ResultText);
        
        mistakeStorageMock.Verify(p => p.SaveMistakeAsync(new WordMistake {
            WordId = wordToReturn.Id,
            IsInNote = true,
        }), Times.Once);
        
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
        var wordQuizViewModel = new WordSelectionViewModel(mockWordStorage, mockContentNavigationService, null, null);

        await Task.Delay(1000);
        wordQuizViewModel.ShowDetail();
        contentNavigationServiceMock.Verify(
            p => p.NavigateTo(
                ContentNavigationConstant.WordDetailView, wordQuizViewModel.CorrectWord),
            Times.Once);
    }
}