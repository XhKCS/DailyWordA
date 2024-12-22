using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace DailyWordA.Library.Models;

// 只有收藏的每日格言才会存到数据库中
[SQLite.Table("favorite_mottos_table")]
public class DailyMotto : ObservableObject {
    [PrimaryKey] [AutoIncrement] public int Id { get; set; }
    
    // private bool _isFavorite;
    // public virtual bool IsFavorite {
    //     get => _isFavorite;
    //     set => SetProperty(ref _isFavorite, value);
    // }
    
    public DateTime FTimestamp { get; set; }
    
    public string Content { get; set; } = string.Empty;
    
    public string Translation { get; set; } = string.Empty; //中文释义
    
    public string Source { get; set; } = string.Empty; //来源，可能为空字符串
    
    public string Date { get; set; } = string.Empty; //当日的日期
    
    public string Author { get; set; } = string.Empty; //作者
}