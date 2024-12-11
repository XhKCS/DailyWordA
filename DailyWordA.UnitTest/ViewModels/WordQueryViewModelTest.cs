using System.Linq.Expressions;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;
using Moq;

namespace DailyWordA.UnitTest.ViewModels;

public class WordQueryViewModelTest {
    [Fact]
    public async Task SetParameter_EnglishWord() {
        var wordQueryViewModel = new WordQueryViewModel(null);
        Assert.Equal(FilterType.EnglishWordFilter,
            wordQueryViewModel.Filter.Type);
        Assert.True(string.IsNullOrWhiteSpace(wordQueryViewModel
            .Filter.QueryText));

        var wordQuery = new WordQuery {
            Word = "navigate", CnMeaning = "v. 导航"
        };
        wordQueryViewModel.SetParameter(wordQuery);
        
        Assert.Equal(FilterType.EnglishWordFilter,
            wordQueryViewModel.Filter.Type);
        Assert.Equal(wordQuery.Word,
            wordQueryViewModel.Filter.QueryText);
    }
    
    [Fact]
    public async Task SetParameter_CnMeaning() {
        var wordQueryViewModel = new WordQueryViewModel(null);
        Assert.Equal(FilterType.EnglishWordFilter,
            wordQueryViewModel.Filter.Type);
        Assert.True(string.IsNullOrWhiteSpace(wordQueryViewModel
            .Filter.QueryText));

        wordQueryViewModel.Filter.Type = FilterType.ChineseMeaningFilter;
        
        var wordQuery = new WordQuery {
            Word = "navigate", CnMeaning = "v. 导航"
        };
        wordQueryViewModel.SetParameter(wordQuery);
        
        Assert.Equal(FilterType.ChineseMeaningFilter,
            wordQueryViewModel.Filter.Type);
        Assert.Equal(wordQuery.CnMeaning,
            wordQueryViewModel.Filter.QueryText);
    }
    
    [Fact]
    public async Task SetParameter_WrongType() {
        var wordQueryViewModel = new WordQueryViewModel(null);
        Assert.Equal(FilterType.EnglishWordFilter,
            wordQueryViewModel.Filter.Type);
        Assert.True(string.IsNullOrWhiteSpace(wordQueryViewModel
            .Filter.QueryText));

        var wrongParameter = "navigate";
        wordQueryViewModel.SetParameter(wrongParameter);
        
        Assert.True(string.IsNullOrWhiteSpace(wordQueryViewModel
            .Filter.QueryText));
    }
    
    [Fact]
    public void Query_Default() {
        var contentNavigationServiceMock = new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;
        var wordQueryViewModel = new WordQueryViewModel(mockContentNavigationService);
        
        wordQueryViewModel.Query();
        contentNavigationServiceMock.Verify(p => p.NavigateTo(ContentNavigationConstant.WordQueryResultView, wordQueryViewModel.Where), 
            Times.Once);
    }
}