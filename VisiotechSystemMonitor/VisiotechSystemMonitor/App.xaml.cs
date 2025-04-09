using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;
using VisiotechSystemMonitor.ViewModels;
using VisiotechSystemMonitorLib.Interfaces;
using VisiotechSystemMonitorLib.Services;

namespace VisiotechSystemMonitor
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public static ServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();

            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            ServiceProvider = _serviceProvider;
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Registrar la configuración
            services.AddSingleton(_configuration);

            // Registrar tus servicios e ViewModels aquí
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<IDataCollectorService, DataCollectorService>();
            services.AddSingleton<IStaticService>(provider =>
            {
                var connectionString = provider.GetRequiredService<IConfiguration>().GetConnectionString("VisiotechMonitorDB");
                return new StaticService(connectionString);
            });

            // Registrar MainWindow
            services.AddTransient<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = ServiceProvider.GetRequiredService<MainViewModel>();
            mainWindow.Show();
        }
    }
}