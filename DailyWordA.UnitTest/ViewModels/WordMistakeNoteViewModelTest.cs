using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;
using Moq;

namespace DailyWordA.UnitTest.ViewModels;

public class WordMistakeNoteViewModelTest {
    [Fact]
    public async Task LoadedCommandFunction_Default() {
        var wordListToReturn = new List<WordObject>();
        for (var i = 1; i <= 5; i++) {
            wordListToReturn.Add(new WordObject {
                Id = i
            });
        }

        var mistakeListToReturn = new List<WordMistake>();
        mistakeListToReturn.AddRange(
            wordListToReturn.Select(p => new WordMistake {
                WordId = p.Id
            }));

        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        mistakeStorageMock.Setup(p => p.GetMistakeListAsync())
            .ReturnsAsync(mistakeListToReturn);
        var mockMistakeStorage = mistakeStorageMock.Object;

        var wordStorageMock = new Mock<IWordStorage>();
        wordListToReturn.ForEach(p =>
            wordStorageMock.Setup(m => m.GetWordAsync(p.Id))
                .ReturnsAsync(p));
        var mockWordStorage = wordStorageMock.Object;

        var mistakeNoteViewModel =
            new WordMistakeNoteViewModel(mockMistakeStorage, mockWordStorage,
                null);

        var loadingList = new List<bool>();
        mistakeNoteViewModel.PropertyChanged += (sender, args) => {
            if (args.PropertyName == nameof(WordMistakeNoteViewModel.IsLoading)) {
                loadingList.Add(mistakeNoteViewModel.IsLoading);
            }
        };

        Assert.Empty(mistakeNoteViewModel.WordMistakeNoteCollection);
        await mistakeNoteViewModel.OnInitializedAsync();
        Assert.Equal(mistakeListToReturn.Count,
            mistakeNoteViewModel.WordMistakeNoteCollection.Count);
        mistakeStorageMock.Verify(p => p.GetMistakeListAsync(), Times.Once);
        mistakeListToReturn.ForEach(p =>
            wordStorageMock.Verify(m => m.GetWordAsync(p.WordId),
                Times.Once));

        for (var i = 0;
             i < mistakeNoteViewModel.WordMistakeNoteCollection.Count;
             i++) {
            Assert.Same(mistakeListToReturn[i],
                mistakeNoteViewModel.WordMistakeNoteCollection[i].WordMistake);
            Assert.Same(wordListToReturn[i],
                mistakeNoteViewModel.WordMistakeNoteCollection[i].WordObject);
        }
    }
    
    [Fact]
    public async Task WordTappedCommandFunction_Default() {
        var contentNavigationServiceMock =
            new Mock<IContentNavigationService>();
        var mockContentNavigationService = contentNavigationServiceMock.Object;

        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        var mockMistakeStorage = mistakeStorageMock.Object;

        var mistakeNoteViewModel =
            new WordMistakeNoteViewModel(mockMistakeStorage, null,
                mockContentNavigationService);
        var wordMistakeToNavigate =
            new WordMistakeNote {
                WordObject = new WordObject()
            };

        mistakeNoteViewModel.ShowWordDetail(
            wordMistakeToNavigate.WordObject);
        contentNavigationServiceMock.Verify(
            p => p.NavigateTo(ContentNavigationConstant.WordDetailView,
                wordMistakeToNavigate.WordObject), Times.Once);
    }
    
    [Fact]
    public void MistakeStorageOnUpdated_Default() {
        var mistakeStorageMock = new Mock<IWordMistakeStorage>();
        var mockMistakeStorage = mistakeStorageMock.Object;

        var wordMistakeList = new List<WordMistakeNote>();
        for (int i = 1; i <= 5; i++) {
            wordMistakeList.Add(new WordMistakeNote {
                WordMistake = new WordMistake {
                    WordId = i, IsInNote = true, Timestamp = DateTime.Now.Subtract(TimeSpan.FromMinutes(i))
                }
            });
        }

        var mistakeUpdated = new WordMistake {
            WordId = wordMistakeList[2].WordMistake.WordId,
            IsInNote = false,
            Timestamp = wordMistakeList[2].WordMistake.Timestamp
        };
        var wordToReturn = new WordObject {
            Id = mistakeUpdated.WordId
        };

        var wordStorageMock = new Mock<IWordStorage>();
        wordStorageMock.Setup(p => p.GetWordAsync(wordToReturn.Id))
            .ReturnsAsync(wordToReturn);
        var mockWordStorage = wordStorageMock.Object;

        var wordMistakeNoteViewModel =
            new WordMistakeNoteViewModel(mockMistakeStorage, mockWordStorage,
                null);
        Assert.Empty(wordMistakeNoteViewModel.WordMistakeNoteCollection);
        wordMistakeNoteViewModel.WordMistakeNoteCollection.AddRange(
            wordMistakeList);

        mistakeStorageMock.Raise(p => p.Updated += null, mockMistakeStorage,
            new MistakeStorageUpdatedEventArgs(mistakeUpdated));
        Assert.Equal(wordMistakeList.Count - 1,
            wordMistakeNoteViewModel.WordMistakeNoteCollection.Count);
        Assert.DoesNotContain(wordMistakeNoteViewModel.WordMistakeNoteCollection, p =>
            p.WordMistake.WordId == mistakeUpdated.WordId);

        mistakeUpdated.IsInNote = true;
        mistakeStorageMock.Raise(p => p.Updated += null, mockMistakeStorage,
            new MistakeStorageUpdatedEventArgs(mistakeUpdated));
        Assert.Equal(wordMistakeList.Count,
            wordMistakeNoteViewModel.WordMistakeNoteCollection.Count);
        Assert.Equal(mistakeUpdated.WordId - 1,
            wordMistakeNoteViewModel.WordMistakeNoteCollection.IndexOf(
                wordMistakeNoteViewModel.WordMistakeNoteCollection.First(p =>
                    p.WordMistake.WordId == mistakeUpdated.WordId)));
    }
}