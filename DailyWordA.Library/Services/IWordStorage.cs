using System.Linq.Expressions;
using DailyWordA.Library.Models;

namespace DailyWordA.Library.Services;

public interface IWordStorage {
    bool IsInitialized { get; }

    Task InitializeAsync();
    
    Task<WordObject> GetWordAsync(int id);
    
    Task<WordObject> GetRandomWordAsync();
    
    Task<IList<WordObject>> GetWordsAsync(
        Expression<Func<WordObject, bool>> where, int skip, int take);
    
    Task SaveWordAsync(WordObject wordObject);
}