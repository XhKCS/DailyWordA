using SQLite;

namespace DailyWordA.Library.Models;

[SQLite.Table("words_table")]
public class WordObject {
    [SQLite.Column("id")]
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    [SQLite.Column("word")]
    [Unique]
    public string Word { get; set; }
    
    [SQLite.Column("accent")]
    public string Accent { get; set; } //发音（音标字符串）
    
    [SQLite.Column("cnMeaning")]
    public string CnMeaning { get; set; } //中文释义
    
    [SQLite.Column("enMeaning")]
    public string EnMeaning { get; set; } //英文释义，小概率为空字符串
    
    [SQLite.Column("sentence")]
    public string Sentence { get; set; } //英文例句，小概率为空字符串
    
    [SQLite.Column("sentenceTrans")]
    public string SentenceTrans { get; set; } //例句的中文翻译，小概率为空字符串
    
    [SQLite.Column("phrase")]
    public string Phrase { get; set; } //相关的短语，可能为空字符串
    
    [SQLite.Column("etyma")]
    public string Etyma { get; set; } //词根，可能为空字符串
}