using Floai.Models;
using Floai.Pages;
using Floai.Utils.View;
using Floai.Utils.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace Floai
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }
        public App()
        {
            string configFilePath = "appsettings.json";
            var config = new ConfigurationBuilder()
                .AddJsonFile(configFilePath, false,true)
                .Build();
            var appSettings = new AppSettings(configFilePath);
            config.GetSection("setting").Bind(appSettings);
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<FloatView>();
                    services.AddSingleton<ChatView>();
                    services.AddTransient<SettingsView>();
                    services.AddSingleton<WindowsTaskbarIcon>();
                    services.AddSingleton<WindowManager>();
                    services.AddSingleton(appSettings);
                }).Build();
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();
            var manager = AppHost.Services.GetRequiredService<WindowManager>();
            var taskbarIcon = AppHost.Services.GetRequiredService<WindowsTaskbarIcon>();
            taskbarIcon.Open();
            manager.SetWindow<FloatView>(null);
            base.OnStartup(e);
        }
        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            base.OnExit(e);
        }
    }
}
