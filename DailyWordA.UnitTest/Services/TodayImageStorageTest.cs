using System.Security.Cryptography;
using System.Text;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;
using Moq;

namespace DailyWordA.UnitTest.Services;

[Collection("ImageFile")]
public class TodayImageStorageTest : IDisposable {
    public TodayImageStorageTest() => RemoveImageFile();

    public void Dispose() => RemoveImageFile();
    
    private static void RemoveImageFile() =>
        File.Delete(TodayImageStorage.TodayImagePath);
    
    [Fact]
    public async Task GetTodayImageAsync_Default() {
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        preferenceStorageMock
            .Setup(p => p.Get(TodayImageStorage.StartDateKey,
                TodayImageStorage.StartDateDefault))
            .Returns(TodayImageStorage.StartDateDefault);
        preferenceStorageMock
            .Setup(p => p.Get(TodayImageStorage.ExpiresAtKey,
                TodayImageStorage.ExpiresAtDefault))
            .Returns(TodayImageStorage.ExpiresAtDefault);
        preferenceStorageMock
            .Setup(p => p.Get(TodayImageStorage.CopyrightKey,
                TodayImageStorage.CopyrightDefault))
            .Returns(TodayImageStorage.CopyrightDefault);
        preferenceStorageMock
            .Setup(p => p.Get(TodayImageStorage.TitleKey,
                TodayImageStorage.TitleDefault))
            .Returns(TodayImageStorage.TitleDefault);
        var mockPreferenceStorage = preferenceStorageMock.Object;

        var todayImageService = new TodayImageStorage(mockPreferenceStorage);
        var todayImage = await todayImageService.GetTodayImageAsync(false);

        Assert.Equal(TodayImageStorage.StartDateDefault, todayImage.StartDate);
        Assert.Equal(TodayImageStorage.ExpiresAtDefault, todayImage.ExpiresAt);
        Assert.Equal(TodayImageStorage.CopyrightDefault, todayImage.Copyright);
        Assert.Equal(TodayImageStorage.TitleDefault, todayImage.Title);
        Assert.Null(todayImage.ImageBytes);

        var md5 = MD5.Create();
        var resourceImageHash = BitConverter.ToString(
            await md5.ComputeHashAsync(
                typeof(TodayImageStorage).Assembly.GetManifestResourceStream(
                    TodayImageStorage.Filename) ??
                throw new NullReferenceException(
                    "Null manifest resource stream")));
        
        todayImage = await todayImageService.GetTodayImageAsync(true);
        Assert.NotNull(todayImage.ImageBytes);
        var todayImageHash =
            BitConverter.ToString(md5.ComputeHash(todayImage.ImageBytes!));

        Assert.Equal(resourceImageHash, todayImageHash);

        File.Delete(TodayImageStorage.TodayImagePath);

        var bytesToWrite =
            Encoding.ASCII.GetBytes(DateTime.Now.Ticks.ToString());
        await using (var todayImageStream =
                     new FileStream(TodayImageStorage.TodayImagePath,
                         FileMode.Create))
        await using (var todayImageWriter =
                     new BinaryWriter(todayImageStream)) {
            todayImageWriter.Write(bytesToWrite);
        }

        todayImage = await todayImageService.GetTodayImageAsync(true);
        Assert.NotNull(todayImage.ImageBytes);
        Assert.True(bytesToWrite.SequenceEqual(todayImage.ImageBytes!));
    }
    
    [Fact]
    public async Task SaveTodayImageAsync_Default() {
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        var mockPreferenceStorage = preferenceStorageMock.Object;

        var todayImageToUpdate = new TodayImage {
            ExpiresAt = DateTime.Now,
            StartDate = DateTime.Today.ToShortDateString(),
            Copyright = "Copyright",
            Title = "Today's image",
            ImageBytes = Encoding.ASCII.GetBytes(DateTime.Now.Ticks.ToString())
        };

        var todayImageStorage = new TodayImageStorage(mockPreferenceStorage);
        await todayImageStorage.SaveTodayImageAsync(todayImageToUpdate, false);

        preferenceStorageMock.Verify(
            p => p.Set(TodayImageStorage.ExpiresAtKey,
                todayImageToUpdate.ExpiresAt), Times.Once);
        preferenceStorageMock.Verify(
            p => p.Set(TodayImageStorage.StartDateKey,
                todayImageToUpdate.StartDate), Times.Once);
        preferenceStorageMock.Verify(
            p => p.Set(TodayImageStorage.CopyrightKey,
                todayImageToUpdate.Copyright), Times.Once);
        preferenceStorageMock.Verify(
            p => p.Set(TodayImageStorage.TitleKey,
                todayImageToUpdate.Title), Times.Once);
        Assert.Equal(todayImageToUpdate.ImageBytes.Length,
            new FileInfo(TodayImageStorage.TodayImagePath).Length);

        todayImageToUpdate.ExpiresAt = DateTime.Now;
        var todayImageFileLastWriteTime =
            new FileInfo(TodayImageStorage.TodayImagePath).LastWriteTime;
        await todayImageStorage.SaveTodayImageAsync(todayImageToUpdate, true);

        preferenceStorageMock.Verify(
            p => p.Set(TodayImageStorage.ExpiresAtKey,
                todayImageToUpdate.ExpiresAt), Times.Once);
        preferenceStorageMock.Verify(
            p => p.Set(TodayImageStorage.StartDateKey,
                todayImageToUpdate.StartDate), Times.Once);
        preferenceStorageMock.Verify(
            p => p.Set(TodayImageStorage.CopyrightKey,
                todayImageToUpdate.Copyright), Times.Once);
        preferenceStorageMock.Verify(
            p => p.Set(TodayImageStorage.TitleKey,
                todayImageToUpdate.Title), Times.Once);
        Assert.Equal(todayImageFileLastWriteTime,
            new FileInfo(TodayImageStorage.TodayImagePath).LastWriteTime);
    }
}