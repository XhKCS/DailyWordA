using System.Linq.Expressions;
using System.Text.Json;
using DailyWordA.Library.Helpers;
using DailyWordA.Library.Models;
using SQLite;

namespace DailyWordA.Library.Services;

public class WordStorage : IWordStorage {
    public const string DbName = "wordsdb.sqlite3";

    // public static readonly string WordDbPath =
    //     Path.Combine(
    //         Environment.GetFolderPath(Environment.SpecialFolder
    //             .LocalApplicationData), DbName);
    public static readonly string WordDbPath =
        PathHelper.GetLocalFilePath(DbName);
    
    // SQLite数据库连接
    private SQLiteAsyncConnection _connection;
    private SQLiteAsyncConnection Connection =>
        _connection ??= new SQLiteAsyncConnection(WordDbPath);
    
    // 文件存储需要借助的接口
    private readonly IPreferenceStorage _preferenceStorage;
    private readonly IAlertService _alertService;


    public WordStorage(IPreferenceStorage preferenceStorage, IAlertService alertService)
    {
        _preferenceStorage = preferenceStorage;
        _alertService = alertService;
    }

    public bool IsInitialized =>
        _preferenceStorage.Get(WordStorageConstant.VersionKey,
            default(int)) == WordStorageConstant.Version;

    private const string Server = "百词斩单词服务器";
    
    // 只在数据库文件还没建立成之前运行；当数据已经从api获取完毕后，之后的初始化就只需迁移数据库文件即可
    public async Task InitializeAsyncForFirstTime() {
        await Connection.CreateTableAsync<WordObject>(); //如果该表已经存在则不会重复创建
        
        // 获取单词列表
        using var httpClient = new HttpClient();
        HttpResponseMessage response;
        try {
            response =
                await httpClient.GetAsync("https://cdn.jsdelivr.net/gh/lyc8503/baicizhan-word-meaning-API/data/list.json");
            response.EnsureSuccessStatusCode();
        } catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.HttpClientErrorTitle,
                ErrorMessageHelper.GetHttpClientError(Server, e.Message));
            return;
        }
        var json = await response.Content.ReadAsStringAsync();
        BaicizhanListResult baicizhanListResult;
        try {
            baicizhanListResult = JsonSerializer.Deserialize<BaicizhanListResult>(json,
                new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new JsonException();
        } catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.JsonDeserializationErrorTitle,
                ErrorMessageHelper.GetJsonDeserializationError(Server,
                    e.Message));
            return;
        }

      
        baicizhanListResult.List[7769] = "may";
        baicizhanListResult.List[7913] = "pacific";
        baicizhanListResult.List[7916] = "Turkey";
        // 逐个获取单词详情，并保存到本地数据库表中
        Console.WriteLine("BaicizhanListResult returns successfully, begin to insert one by one.");
        List<WordObject> wordObjects = new List<WordObject>();
        for (var i = 0; i < 10926; i++) {
            var word = baicizhanListResult.List[i];
            if (word.Contains(' ') || word.Contains('/') || word.Contains('_')) {
                continue;
            }
            if (i == 7609) {
                continue;
            }
            HttpResponseMessage response2 = 
                await httpClient.GetAsync($"https://cdn.jsdelivr.net/gh/lyc8503/baicizhan-word-meaning-API/data/words/{word}.json");
            response2.EnsureSuccessStatusCode();
            
            var json2 = await response2.Content.ReadAsStringAsync();
            BaicizhanWordResult baicizhanWordResult = JsonSerializer.Deserialize<BaicizhanWordResult>(json2,
                new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new JsonException();
            WordObject wordObject = new WordObject {
                Word = baicizhanWordResult.word,
                Accent = baicizhanWordResult.accent,
                CnMeaning = baicizhanWordResult.mean_cn,
                EnMeaning = baicizhanWordResult.mean_en,
                Sentence = baicizhanWordResult.sentence,
                SentenceTrans = baicizhanWordResult.sentence_trans,
                Phrase = baicizhanWordResult.sentence_phrase,
                Etyma = baicizhanWordResult.word_etyma
            };
            wordObjects.Add(wordObject);
            Console.WriteLine($"{i+1}: {wordObject.Word}");
            // await Connection.InsertAsync(wordObject);
            if ((i + 1) % 50 == 0) {
                // await Connection.InsertAllAsync(wordObjects);
                wordObjects.Clear();
                Console.WriteLine("Inserted---");
            }
        }
        
        _preferenceStorage.Set(WordStorageConstant.VersionKey,
            WordStorageConstant.Version);

        await Connection.CloseAsync();
        Console.WriteLine("WordStorage initialization complete.");
    }

    
    public async Task InitializeAsync() {
        // await InitializeAsyncForFirstTime();
        
        //单词数据已经通过api整理保存到了数据库文件中；在此只需获取资源文件中的数据库文件，并将其复制到客户机的目标文件上
        await using var dbFileStream =
            new FileStream(WordDbPath, FileMode.OpenOrCreate);
        
        // 加上using可以让该函数结束时自动关闭数据库连接
        await using var dbAssetStream =
            typeof(WordStorage).Assembly.GetManifestResourceStream(DbName); //不能直接用DbName来找，前面还有前缀，否则找不到
        //因为资源只能用流打开，所以我们用流对流拷贝
        if (dbAssetStream != null) await dbAssetStream.CopyToAsync(dbFileStream);
        
        //设置当前数据库版本
        _preferenceStorage.Set(WordStorageConstant.VersionKey,
            WordStorageConstant.Version); 
    }

    public async Task<WordObject> GetWordAsync(int id) =>
        await Connection.Table<WordObject>()
            .FirstOrDefaultAsync(p => p.Id == id);
    
    // 从本地数据库随机获取一个单词，不能是空
    public async Task<WordObject> GetRandomWordAsync() {
        var words = await GetWordsAsync(
            Expression.Lambda<Func<WordObject, bool>>(Expression.Constant(true),
                Expression.Parameter(typeof(WordObject), "p")),
            new Random().Next(5000), 1);
        var word = words.First();
        return word;
    }

    public async Task<IList<WordObject>> GetWordsAsync(
        Expression<Func<WordObject, bool>> where, int skip, int take)
    {
        return await Connection.Table<WordObject>().Where(where).Skip(skip).Take(take)
            .ToListAsync();
    }

    public async Task SaveWordAsync(WordObject wordObject)
    {
        await Connection.InsertOrReplaceAsync(wordObject);
    }

    public async Task<IList<WordObject>> GetWordQuizOptionsAsync(WordObject correctWord) {
        Random random = new Random();
        List<WordObject> wordList = await Connection.Table<WordObject>().Where(
                p=>p.Word != correctWord.Word).Skip(random.Next(5000)).Take(3)
            .ToListAsync();
        var randomIndex = random.Next(0,3);
        wordList.Insert(randomIndex, correctWord);
        return wordList;
    }

    //在实现类中提供一个数据库关闭函数；为什么不放在接口中：因为它与业务无关
    public async Task CloseAsync() => await Connection.CloseAsync();
}

public static class WordStorageConstant {
    public const string VersionKey =
        nameof(WordStorageConstant) + "." + nameof(Version);

    public const int Version = 1;
}

public class BaicizhanListResult
{
    public int Total { get; set; }
    public string[] List { get; set; }
}

public class BaicizhanWordResult
{
    public string word { get; set; }
    public string accent { get; set; }
    public string mean_cn { get; set; }
    public string mean_en { get; set; }
    public string sentence { get; set; }
    public string sentence_trans { get; set; }
    public string sentence_phrase { get; set; }
    public string word_etyma { get; set; }
    public ClozeData cloze_data { get; set; }
}

public class ClozeData
{
    public string syllable { get; set; }
    public string cloze { get; set; }
    public string[] options { get; set; }
    public string[][] tips { get; set; }
}
