using DailyWordA.Library.Models;
using DailyWordA.Library.ViewModels;

namespace DailyWordA.UnitTest.ViewModels;

public class WordQueryViewModelTest {
    [Fact]
    public async Task SetParameter_Default() {
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
}