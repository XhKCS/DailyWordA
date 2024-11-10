namespace DailyWordA.Library.Services;

public interface ITranslateService {
    Task<string> Translate(string sourceText, string from="auto", string to="zh");
}