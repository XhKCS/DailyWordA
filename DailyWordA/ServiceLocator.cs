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
    public TodayWordViewModel TodayWordViewModel => 
        _serviceProvider.GetRequiredService<TodayWordViewModel>();
    
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
    
    public WordDetailViewModel WordDetailViewModel =>
        _serviceProvider.GetRequiredService<WordDetailViewModel>();
    
    public MottoDetailViewModel MottoDetailViewModel =>
        _serviceProvider.GetRequiredService<MottoDetailViewModel>();
    
    public InitializationViewModel InitializationViewModel =>
        _serviceProvider.GetRequiredService<InitializationViewModel>();
    
    public WordQueryViewModel WordQueryViewModel =>
        _serviceProvider.GetRequiredService<WordQueryViewModel>();
    
    public WordQueryResultViewModel WordQueryResultViewModel =>
        _serviceProvider.GetRequiredService<WordQueryResultViewModel>();
    
    public WordFavoriteViewModel WordFavoriteViewModel =>
        _serviceProvider.GetRequiredService<WordFavoriteViewModel>();
    
    public WordQuizViewModel WordQuizViewModel =>
        _serviceProvider.GetRequiredService<WordQuizViewModel>();
    
    public ServiceLocator() {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<IPreferenceStorage, FilePreferenceStorage>();
        serviceCollection.AddSingleton<IWordStorage, WordStorage>();
        serviceCollection.AddSingleton<IDailyMottoService, DailyMottoService>();
        serviceCollection.AddSingleton<IAlertService, AlertService>();
        serviceCollection.AddSingleton<IRootNavigationService, RootNavigationService>();
        serviceCollection.AddSingleton<IMenuNavigationService, MenuNavigationService>();
        serviceCollection.AddSingleton<IContentNavigationService, ContentNavigationService>();
        serviceCollection.AddSingleton<ITranslateService, TranslateService>();
        serviceCollection.AddSingleton<ITodayImageService, TodayImageService>();
        serviceCollection.AddSingleton<ITodayImageStorage, TodayImageStorage>();
        serviceCollection.AddSingleton<IWordFavoriteStorage, WordFavoriteStorage>();
        
        serviceCollection.AddSingleton<MainWindowViewModel>();
        serviceCollection.AddSingleton<TodayWordViewModel>();
        serviceCollection.AddSingleton<TodayMottoViewModel>();
        serviceCollection.AddSingleton<MainViewModel>();
        serviceCollection.AddSingleton<TranslateViewModel>();
        serviceCollection.AddSingleton<WordDetailViewModel>();
        serviceCollection.AddSingleton<MottoDetailViewModel>();
        serviceCollection.AddSingleton<InitializationViewModel>();
        serviceCollection.AddSingleton<WordQueryViewModel>();
        serviceCollection.AddSingleton<WordQueryResultViewModel>();
        serviceCollection.AddSingleton<WordFavoriteViewModel>();
        serviceCollection.AddSingleton<WordQuizViewModel>();

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }
    
}