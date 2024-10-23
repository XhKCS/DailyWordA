using System.Threading.Tasks;
using DailyWordA.Library.Services;
using Ursa.Controls;

namespace DailyWordA.Services;

public class AlertService : IAlertService {
    public async Task AlertAsync(string title, string message) =>
        await MessageBox.ShowAsync(message, title, button: MessageBoxButton.OK);
}