using LiveVol.UI.Service;

using Microsoft.Extensions.DependencyInjection;

using OpenQA.Selenium.Edge;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using Microsoft.Extensions.Configuration;

namespace LiveVol.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }
        private readonly IServiceProvider _serviceProvider;
        private void ConfigureServices(IServiceCollection services)
        {
            var cfg = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<Browser>();
            services.AddSingleton<IConfiguration>(cfg);
            var options = new EdgeOptions();
            options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36 Edg/114.0.1823.67");
            options.AddArgument("--headless=new");
            options.AddArgument("--window-size=2560,3000");
            services.AddSingleton(new EdgeDriver(options));
            services.AddSingleton<List<LiveVolData>>();
        }
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
