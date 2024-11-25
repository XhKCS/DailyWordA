using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;
using Moq;

namespace DailyWordA.UnitTest.ViewModels;

public class TodayWordViewModelTest {
    [Fact]
    public async Task OnInitialized_ImageUpdated() {
        var oldTodayImageToReturn = new TodayImage();
        var newTodayImageToReturn = new TodayImage();
        var updateResultToReturn = new TodayImageServiceCheckUpdateResult {
            HasUpdate = true, TodayImage = newTodayImageToReturn
        };
        
        var todayImageServiceMock = new Mock<ITodayImageService>();
        todayImageServiceMock.Setup(p => p.GetTodayImageAsync())
            .ReturnsAsync(oldTodayImageToReturn);
        todayImageServiceMock.Setup(p => p.CheckUpdateAsync())
            .ReturnsAsync(updateResultToReturn);
        var mockImageService = todayImageServiceMock.Object;

        var todayWordToReturn = new WordObject();
        var wordStorageMock = new Mock<IWordStorage>();
        wordStorageMock.Setup(p => p.GetRandomWordAsync())
            .ReturnsAsync(todayWordToReturn);
        var mockWordStorage = wordStorageMock.Object;
        
        // 注意构造函数中已经自动调用了OnInitialized方法
        var todayWordViewModel = new TodayWordViewModel(mockWordStorage, mockImageService, null, null);
        
        var todayImageList = new List<TodayImage>();
        var isLoadingList = new List<bool>();
        todayWordViewModel.PropertyChanged += (sender, args) => {
            switch (args.PropertyName) {
                case nameof(TodayWordViewModel.TodayImage):
                    todayImageList.Add(todayWordViewModel.TodayImage);
                    break;
                case nameof(TodayWordViewModel.IsLoading):
                    isLoadingList.Add(todayWordViewModel.IsLoading);
                    break;
            }
        };
        
        
        while (todayImageList.Count != 2 || isLoadingList.Count != 2) {
            await Task.Delay(100);
        }
        
        Assert.Same(oldTodayImageToReturn, todayImageList[0]);
        Assert.Same(newTodayImageToReturn, todayImageList[1]);
        Assert.True(isLoadingList[0]);
        Assert.False(isLoadingList[1]);
        Assert.Same(todayWordToReturn, todayWordViewModel.TodayWord);

        todayImageServiceMock.Verify(p => p.GetTodayImageAsync(), Times.Once);
        todayImageServiceMock.Verify(p => p.CheckUpdateAsync(), Times.Once);
        wordStorageMock.Verify(p => p.GetRandomWordAsync(), Times.Once);
    }
}