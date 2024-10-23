using Avalonia.Controls;
using DailyWordA.Library.ViewModels;

namespace DailyWordA.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        //调用RootNavigationService
        // ServiceLocator.Current.RootNavigationService.NavigateTo(
        //     nameof(WordResultViewModel));
        // ServiceLocator.Current.RootNavigationService.NavigateTo(
        //     nameof(TodayMottoViewModel));
    }
}