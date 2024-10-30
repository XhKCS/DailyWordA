using DailyWordA.Library.Helpers;
using DailyWordA.Library.Models;

namespace DailyWordA.Library.Services;

public class TodayImageStorage : ITodayImageStorage {
    private readonly IPreferenceStorage _preferenceStorage;

    public TodayImageStorage(IPreferenceStorage preferenceStorage) {
        _preferenceStorage = preferenceStorage;
    }

    public static readonly string StartDateKey = nameof(TodayImageStorage) +
        "." + nameof(TodayImage.StartDate);

    public static readonly string ExpiresAtKey =
        nameof(TodayImageStorage) + "." + nameof(TodayImage.ExpiresAt);

    public static readonly string CopyrightKey =
        nameof(TodayImageStorage) + "." + nameof(TodayImage.Copyright);
    
    public static readonly string TitleKey =
        nameof(TodayImageStorage) + "." + nameof(TodayImage.Title);

    public const string StartDateDefault = "20241028";

    public static readonly DateTime ExpiresAtDefault = new(2024, 10, 29, 16, 0, 0);

    public const string CopyrightDefault = "大雕鸮 (© Mark Newman/Getty Images)";

    public const string TitleDefault = "猫头鹰的叫声在萦绕";

    public const string Filename = "todayImage.bin";

    public static readonly string TodayImagePath =
        PathHelper.GetLocalFilePath(Filename);


    public async Task<TodayImage> GetTodayImageAsync(
        bool isIncludingImageStream) {
        var todayImage = new TodayImage {
            StartDate =
                _preferenceStorage.Get(StartDateKey, StartDateDefault),
            ExpiresAt = _preferenceStorage.Get(ExpiresAtKey, ExpiresAtDefault),
            Copyright = _preferenceStorage.Get(CopyrightKey, CopyrightDefault),
            Title = _preferenceStorage.Get(TitleKey, TitleDefault),
        };

        if (!File.Exists(TodayImagePath)) {
            await using var imageAssetFileStream =
                new FileStream(TodayImagePath, FileMode.Create) ??
                throw new NullReferenceException("Null file stream.");
            await using var imageAssetStream =
                typeof(TodayImageStorage).Assembly.GetManifestResourceStream(
                    Filename) ??
                throw new NullReferenceException(
                    "Null manifest resource stream");
            await imageAssetStream.CopyToAsync(imageAssetFileStream);
        }

        if (!isIncludingImageStream) {
            return todayImage;
        }

        var imageMemoryStream = new MemoryStream();
        await using var imageFileStream =
            new FileStream(TodayImagePath, FileMode.Open);
        // 文件流不能直接转为字节数组，只能先转为内存流，再转为字节数组
        await imageFileStream.CopyToAsync(imageMemoryStream);
        todayImage.ImageBytes = imageMemoryStream.ToArray();

        return todayImage;
    }

    public async Task SaveTodayImageAsync(TodayImage todayImage,
        bool isSavingExpiresAtOnly) {
        _preferenceStorage.Set(ExpiresAtKey, todayImage.ExpiresAt);
        if (isSavingExpiresAtOnly) {
            return;
        }

        if (todayImage.ImageBytes == null) {
            throw new ArgumentException($"Null image bytes.",
                nameof(todayImage));
        }

        _preferenceStorage.Set(StartDateKey, todayImage.StartDate);
        _preferenceStorage.Set(ExpiresAtKey, todayImage.ExpiresAt);
        _preferenceStorage.Set(CopyrightKey, todayImage.Copyright);
        _preferenceStorage.Set(TitleKey, todayImage.Title);

        await using var imageFileStream =
            new FileStream(TodayImagePath, FileMode.Create);
        await imageFileStream.WriteAsync(todayImage.ImageBytes, 0,
            todayImage.ImageBytes.Length);
    }

    public async Task TrySaveDefaultImageAsync() {
        TodayImage dafaultImage = new TodayImage {
            StartDate = StartDateDefault,
            ExpiresAt = ExpiresAtDefault,
            Copyright = CopyrightDefault,
            Title = TitleDefault,
        };
        
        HttpClient httpClient = new();
        HttpResponseMessage response;
        try {
            response =
                await httpClient.GetAsync(
                    "https://www.bing.com/th?id=OHR.GreatOwl_ZH-CN1259534922_1920x1080.jpg&rf=LaDigue_1920x1080.jpg&pid=hp");
            response.EnsureSuccessStatusCode();
        } catch (Exception e) {
            Console.WriteLine(e.Message);
            return;
        }
        dafaultImage.ImageBytes = await response.Content.ReadAsByteArrayAsync();
        
        await SaveTodayImageAsync(dafaultImage, false);
    }
}