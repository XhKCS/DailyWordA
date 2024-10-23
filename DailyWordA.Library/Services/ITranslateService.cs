namespace DailyWordA.Library.Services;

public interface ITranslateService {
    Task<string> Translate(string sourceText);
}