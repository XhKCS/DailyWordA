using System.Linq.Expressions;
using DailyWordA.Library.Models;

namespace DailyWordA.Library.Services;

public interface IWordStorage {
    bool IsInitialized { get; }

    Task InitializeAsync();

    Task InitializeAsyncForFirstTime(); //只在第一次获取单词保存为数据库表时用到
    
    Task<WordObject> GetWordAsync(int id);
    
    Task<WordObject> GetRandomWordAsync();
    
    Task<IList<WordObject>> GetWordsAsync(
        Expression<Func<WordObject, bool>> where, int skip, int take);
    
    Task SaveWordAsync(WordObject wordObject);
    
    // 获取与correctWord不相同的三个单词，四个单词一起作为List返回，作为单词测验的选项
    Task<IList<WordObject>> GetWordQuizOptionsAsync(WordObject correctWord);
}