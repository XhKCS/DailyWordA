using System.Linq.Expressions;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyWordA.Library.Models;
using DailyWordA.Library.Services;

namespace DailyWordA.Library.ViewModels;

public class WordQueryViewModel : ViewModelBase {
    private readonly IContentNavigationService _contentNavigationService;

    public WordQueryViewModel(IContentNavigationService contentNavigationService) {
        _contentNavigationService = contentNavigationService;
        QueryCommand = new RelayCommand(Query);
    }

    private FilterViewModel _filter = new();
    public FilterViewModel Filter {
        get => _filter;
        set => SetProperty(ref _filter, value);
    }

    private string _commentText = "请输入想要查询的英文单词或中文释义";
    public string CommentText {
        get => _commentText;
        private set => SetProperty(ref _commentText, value);
    }
    
    public override void SetParameter(object parameter) {
        if (parameter is not WordQuery wordQuery) {
            return;
        }

        if (Filter.Type == FilterType.EnglishWordFilter) {
            Filter.QueryText = wordQuery.Word;
        }
        else {
            Filter.QueryText = wordQuery.CnMeaning;
        }
    }
    
    public ICommand QueryCommand { get; }

    public void Query() {
        var parameter = Expression.Parameter(typeof(WordObject), "p");
        var expression = GetExpression(parameter, _filter);
        var where = 
            Expression.Lambda<Func<WordObject, bool>>(expression, parameter);
        _contentNavigationService.NavigateTo(ContentNavigationConstant.WordQueryResultView, where);
    }
    
    private static Expression GetExpression(ParameterExpression parameter,
        FilterViewModel filterViewModel) {
        // parameter => p

        // p.Word or p.CnMeaning
        var property = Expression.Property(parameter,
            filterViewModel.Type.PropertyName);

        // .Contains()
        var method =
            typeof(string).GetMethod("Contains", new[] {
                typeof(string)
            });

        // "something"
        var condition =
            Expression.Constant(filterViewModel.QueryText.Replace("\n", "").Replace(" ",""), typeof(string));

        // p.Word.Contains("something")
        // or p.CnMeaning.Contains("something")
        return Expression.Call(property, method, condition);
    }
    
}

public class FilterViewModel : ObservableObject {
    private string _queryText = string.Empty;
    public string QueryText {
        get => _queryText;
        set => SetProperty(ref _queryText, value);
    }
    
    private FilterType _type = FilterType.EnglishWordFilter;
    public FilterType Type {
        get => _type;
        set => SetProperty(ref _type, value);
    }
}

public class FilterType {
    public static readonly FilterType EnglishWordFilter =
        new("按英文查找", nameof(WordObject.Word));

    public static readonly FilterType ChineseMeaningFilter =
        new("按中文释义查找", nameof(WordObject.CnMeaning));
    
    public static List<FilterType> FilterTypes { get; } =
        [EnglishWordFilter, ChineseMeaningFilter];
    
    private FilterType(string name, string propertyName) {
        Name = name;
        PropertyName = propertyName;
    }
    public string Name { get; }
    public string PropertyName { get; }
}