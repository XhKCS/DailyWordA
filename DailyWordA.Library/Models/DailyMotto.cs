using SQLite;

namespace DailyWordA.Library.Models;

// 每日格言直接通过api获取，不存到数据库中；只有收藏的格言（另一个模型类）才会存到数据库中
public class DailyMotto {
    public string Content { get; set; } = string.Empty;
    
    public string Translation { get; set; } = string.Empty; //中文释义
    
    public string Source { get; set; } = string.Empty; //来源，可能为空字符串
    
    public string Date { get; set; } = string.Empty; //当日的日期
    
    public string Author { get; set; } = string.Empty; //作者
}