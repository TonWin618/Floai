using Floai.ApiClients.abs;
using Floai.Models;
using Floai.Pages;
using Floai.Utils.Client;
using Floai.Utils.Model;
using Floai.Utils.View;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
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
            // Load configuration file.
            string configFilePath = "appsettings.json";
            var settingsManager = new SettingsManager(configFilePath);

            var config = new ConfigurationBuilder()
                .AddJsonFile(configFilePath, false, false)
                .Build();

            // Create an instance and bind the configuration options to it.
            var generalSettings = new GeneralSettings();
            config.GetSection("general").Bind(generalSettings);

            // Enable dynamic updating of the configuration file when its settings change.
            generalSettings.isIinitialized = true;
            generalSettings.PropertyChanged += (sender, e) =>
            {
                settingsManager.SaveNode(sender, "general");
            };

            string apiClientName = generalSettings.ApiClientName;

            ApiClientFinder finder = new("Floai.ApiClients");

            // Obtain the desired ApiClient class and ApiClientOptions class through reflection.
            var apiClientOptionsClass = finder.GetTargetApiClientOptionsClass(apiClientName);
            var apiClientClass = finder.GetTargetApiClientClass(apiClientName);

            // Create an instance and bind the configuration options to it.
            var apiClientOptions = Activator.CreateInstance(apiClientOptionsClass);
            config.GetSection("apiClientOptions").GetSection(apiClientName).Bind(apiClientOptions);

            // Obtain all classes of ApiClientOptions through reflection.
            List<Type> apiClientOptionsClasses = finder.GetApiClientOptionsClasses();
            List<BaseApiClientOptions> apiClientOptionses = new();

            // Create an instance and bind the configuration options to it.
            foreach (Type type in apiClientOptionsClasses)
            {
                apiClientOptionses.Add(Activator.CreateInstance(type) as BaseApiClientOptions);
                var clientName = type.Name.Replace("ApiClientOptions", "");
                config.GetSection("apiClientOptions")
                    .GetSection(clientName)
                    .Bind(apiClientOptionses
                    .Single(
                        options => options.GetType().Name == clientName + "ApiClientOptions")
                    );
            }

            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<FloatView>();
                    services.AddSingleton<ChatView>();
                    services.AddTransient<SettingsView>();

                    services.AddSingleton<FloatViewModel>();
                    services.AddSingleton<ChatViewModel>();
                    services.AddTransient<SettingsViewModel>();

                    services.AddSingleton<WindowsTaskbarIcon>();
                    services.AddSingleton<WindowManager>();

                    services.AddSingleton(generalSettings);
                    services.AddSingleton(settingsManager);

                    services.AddSingleton(apiClientOptionsClass ,apiClientOptions);
                    services.AddSingleton(typeof(BaseApiClient), apiClientClass);

                    apiClientOptionses.ForEach(options => services.AddSingleton<BaseApiClientOptions>(options));
                }).Build();
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            // Create a taskbar icon on application startup.
            var taskbarIcon = AppHost.Services.GetRequiredService<WindowsTaskbarIcon>();
            taskbarIcon.Open();

            // Open the floating button on application startup.
            var manager = AppHost.Services.GetRequiredService<WindowManager>();
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
