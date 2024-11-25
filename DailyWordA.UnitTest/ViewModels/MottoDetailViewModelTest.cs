using DailyWordA.Library.Models;
using DailyWordA.Library.ViewModels;

namespace DailyWordA.UnitTest.ViewModels;

public class MottoDetailViewModelTest {
    [Fact]
    public void SetParameter_Default() {
        var todayMotto = new DailyMotto {
            Content = "Once you surrender to your vision, success begins to chase you."
        };

        var todayMottoViewModel = new MottoDetailViewModel(null);
        todayMottoViewModel.SetParameter(todayMotto);
        Assert.Equal(todayMottoViewModel.TodayMotto, todayMotto);
    }
}