using System;
using Avalonia;
using DailyWordA.Library.Services;
using DailyWordA.Library.ViewModels;
using DailyWordA.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DailyWordA;

public class ServiceLocator {
    private readonly IServiceProvider _serviceProvider;

    private static ServiceLocator _current;
    
    // 获取ServiceLocator的实例
    public static ServiceLocator Current {
        get
        {
            if (_current is not null) {
                return _current;
            }

            if (Application.Current.TryGetResource(nameof(ServiceLocator),
                    null, out var resource) &&
                resource is ServiceLocator serviceLocator) {
                return _current = serviceLocator;
            }

            throw new Exception("理论上来讲不应该发生这种情况。");
        }
    }
    
    //对外暴露
    public WordResultViewModel WordResultViewModel => 
        _serviceProvider.GetRequiredService<WordResultViewModel>();
    
    public MainWindowViewModel MainWindowViewModel =>
        _serviceProvider.GetRequiredService<MainWindowViewModel>();
    
    public TodayMottoViewModel TodayMottoViewModel =>
        _serviceProvider.GetRequiredService<TodayMottoViewModel>();
    
    // TODO Delete this
    public IRootNavigationService RootNavigationService =>
        _serviceProvider.GetRequiredService<IRootNavigationService>();
    
    public MainViewModel MainViewModel =>
        _serviceProvider.GetRequiredService<MainViewModel>();
    
    public TranslateViewModel TranslateViewModel =>
        _serviceProvider.GetRequiredService<TranslateViewModel>();
    
    public ServiceLocator() {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<IPreferenceStorage, FilePreferenceStorage>();
        serviceCollection.AddSingleton<IWordStorage, WordStorage>();
        serviceCollection.AddSingleton<IDailyMottoService, DailyMottoService>();
        serviceCollection.AddSingleton<IAlertService, AlertService>();
        serviceCollection.AddSingleton<IRootNavigationService, RootNavigationService>();
        serviceCollection.AddSingleton<IMenuNavigationService, MenuNavigationService>();
        serviceCollection.AddSingleton<ITranslateService, TranslateService>();
        
        serviceCollection.AddSingleton<MainWindowViewModel>();
        serviceCollection.AddSingleton<WordResultViewModel>();
        serviceCollection.AddSingleton<TodayMottoViewModel>();
        serviceCollection.AddSingleton<MainViewModel>();
        serviceCollection.AddSingleton<TranslateViewModel>();
        

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }
    
}