using Floai.ApiClients.abs;
using Floai.Models;
using Floai.Pages;
using Floai.Utils.Client;
using Floai.Utils.View;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
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
                .AddJsonFile(configFilePath, false, false)
                .Build();

            var appSettings = new AppSettings(configFilePath);
            config.GetSection("normal").Bind(appSettings);
            appSettings.isIinitialized = true;


            string apiClientName = appSettings.ApiClientName;

            ApiClientFinder finder = new("Floai.ApiClients");

            Type apiClientOptionsClass = finder.GetApiClientOptionsClass(apiClientName);
            var apiClientOptions = Activator.CreateInstance(apiClientOptionsClass) as BaseApiClientOptions;
            config.GetSection("apiClientOptions").GetSection(apiClientName).Bind(apiClientOptions);

            Type apiClientClass = finder.GetApiClientClass(apiClientName);
            var apiClient = Activator.CreateInstance(apiClientClass, apiClientOptions) as BaseApiClient;

            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<FloatView>();
                    services.AddSingleton<ChatView>();
                    services.AddTransient<SettingsView>();
                    services.AddSingleton<WindowsTaskbarIcon>();
                    services.AddSingleton<WindowManager>();
                    services.AddSingleton(appSettings);
                    services.AddSingleton(apiClient);
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
