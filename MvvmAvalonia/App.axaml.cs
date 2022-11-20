using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CustomerApp.ViewModel;
using CustomerLib;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MvvmAvalonia
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            Services = ConfigureServices();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }

        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; private set; }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ICustomerRepository, CustomerRepository>();
            services.AddSingleton<MainViewModel>();

            return services.BuildServiceProvider();
        }

        public MainViewModel MainVM => Services.GetService<MainViewModel>();
    }
}