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
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;
        
        var wordQuizViewModel = new WordSelectionViewModel(wordStorage, mockContentNavigationService, null);
        await Task.Delay(1000);
        
        Assert.Equal(4, wordQuizViewModel.QuizOptions.Count);
        var oldWord = wordQuizViewModel.CorrectWord;
        Assert.NotNull(oldWord);
        Assert.Contains(oldWord, wordQuizViewModel.QuizOptions);
        
        wordQuizViewModel.Update();
        await Task.Delay(1000);
        
        Assert.NotEqual(oldWord, wordQuizViewModel.CorrectWord);
        // await wordStorage.CloseAsync();
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
        
        var wordQuizViewModel = new WordSelectionViewModel(mockWordStorage, mockContentNavigationService, null);
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
        
        var wordQuizViewModel = new WordSelectionViewModel(mockWordStorage, mockContentNavigationService, null);
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
        
        var wordQuizViewModel = new WordSelectionViewModel(mockWordStorage, mockContentNavigationService, mockMistakeStorage);
        await Task.Delay(1000);
        
        wordQuizViewModel.RadioChecked(wordQuizViewModel.CorrectWord);
        await wordQuizViewModel.CommitAsync();
        
        Assert.True(wordQuizViewModel.HasAnswered);
        Assert.Equal("恭喜您回答正确！", wordQuizViewModel.ResultText);
        // await wordStorage.CloseAsync();
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
        
        var wordQuizViewModel = new WordSelectionViewModel(mockWordStorage, mockContentNavigationService, mockMistakeStorage);
        await Task.Delay(1000);

        var index = wordQuizViewModel.QuizOptions.IndexOf(wordQuizViewModel.CorrectWord);
        var wrongWord = wordQuizViewModel.QuizOptions[(index + 1) % 4];
        wordQuizViewModel.RadioChecked(wrongWord);
        await wordQuizViewModel.CommitAsync();
        
        Assert.True(wordQuizViewModel.HasAnswered);
        Assert.Equal("很遗憾，回答错误啦~", wordQuizViewModel.ResultText);
        
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
        var wordQuizViewModel = new WordSelectionViewModel(mockWordStorage, mockContentNavigationService, null);

        await Task.Delay(1000);
        wordQuizViewModel.ShowDetail();
        contentNavigationServiceMock.Verify(
            p => p.NavigateTo(
                ContentNavigationConstant.WordDetailView, wordQuizViewModel.CorrectWord),
            Times.Once);
    }
}