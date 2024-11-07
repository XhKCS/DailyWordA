using CommunityToolkit.Mvvm.ComponentModel;

namespace DailyWordA.Library.ViewModels;

public class ViewModelBase : ObservableObject {
    // 该函数声明为virtual，可以被子类重写
    public virtual void SetParameter(object parameter) { }
}