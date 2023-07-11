﻿using LiveVol.UI.Service;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenQA.Selenium.Edge;

using System;
using System.Collections.Generic;
using System.Windows;

using Serilog;

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
            var logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
                .WriteTo.Debug()
#endif
#if !DEBUG
                .MinimumLevel.Warning()
#endif 
                .WriteTo.Console()
                .WriteTo.File("Logs/log.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var cfg = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build();

            services.AddLogging(bld => bld.AddSerilog(logger));
            services.AddSingleton(e => new MainWindow(e.GetService<Browser>(), e.GetService<HashSet<LiveVolData>>()));
            services.AddSingleton<Browser>();
            services.AddSingleton<IConfiguration>(cfg);

            var eds = EdgeDriverService.CreateDefaultService();
#if !DEBUG
            eds.HideCommandPromptWindow = true;
#endif
            services.AddSingleton(new EdgeDriver(eds, CreateOptions()));
            services.AddSingleton<HashSet<LiveVolData>>();
        }

        private static EdgeOptions CreateOptions()
        {
            var options = new EdgeOptions();
            options.AddArgument(
                "--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36 Edg/114.0.1823.67");
            options.AddArgument("--headless=new");
            options.AddArgument("--window-size=2560,3000");
            return options;
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
