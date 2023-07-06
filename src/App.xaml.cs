using Floai.Models;
using Floai.Pages;
using Floai.Utils.View;
using Floai.Utils.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using System.Text.Json;
using System.IO;
using Floai.Utils.Client;
using System;

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
            ApiClientFinder finder = new("Floai.ApiClients");
            Type apiClientClass = finder.GetApiClientClass("OpenAi");
            Type apiClientOptionsClass = finder.GetApiClientOptionsClass("OpenAi");

            string configFilePath = "appsettings.json";
            var config = new ConfigurationBuilder()
                .AddJsonFile(configFilePath, false, false)
                .Build();

            object apiClient = Activator.CreateInstance(apiClientClass);
            object apiClientOptions = Activator.CreateInstance(apiClientOptionsClass);
            var appSettings = new AppSettings(configFilePath);

            config.GetSection("apiClientOptions").GetSection("OpenAi").Bind(apiClientOptions);
            config.GetSection("normal").Bind(appSettings);

            appSettings.isIinitialized = true;

            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<FloatView>();
                    services.AddSingleton<ChatView>();
                    services.AddTransient<SettingsView>();
                    services.AddSingleton<WindowsTaskbarIcon>();
                    services.AddSingleton<WindowManager>();
                    services.AddSingleton(appSettings);
                    services.AddSingleton(apiClientClass, apiClient);
                    services.AddSingleton(apiClientOptionsClass, apiClientOptions);
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
