﻿using Floai.ApiClients.abs;
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
            string configFilePath = "appsettings.json";
            var settingsManager = new SettingsManager(configFilePath);

            var config = new ConfigurationBuilder()
                .AddJsonFile(configFilePath, false, false)
                .Build();

            var generalSettings = new GeneralSettings();
            config.GetSection("general").Bind(generalSettings);

            generalSettings.isIinitialized = true;
            generalSettings.PropertyChanged += (sender, e) =>
            {
                settingsManager.SaveNode(sender, "general");
            };

            string apiClientName = generalSettings.ApiClientName;

            ApiClientFinder finder = new("Floai.ApiClients");

            var apiClientOptionsClass = finder.GetTargetApiClientOptionsClass(apiClientName);
            var apiClientOptions = Activator.CreateInstance(apiClientOptionsClass);
            config.GetSection("apiClientOptions").GetSection(apiClientName).Bind(apiClientOptions);

            var apiClientClass = finder.GetTargetApiClientClass(apiClientName);

            List<Type> apiClientOptionsClasses = finder.GetApiClientOptionsClasses();
            List<BaseApiClientOptions> apiClientOptionses = new();
            foreach (Type type in apiClientOptionsClasses)
            {
                apiClientOptionses.Add(Activator.CreateInstance(type) as BaseApiClientOptions);
                var clientName = type.Name.Replace("ApiClientOptions", "");
                config.GetSection("apiClientOptions")
                    .GetSection(clientName)
                    .Bind(apiClientOptionses
                    .Where(
                        options => options.GetType().Name == clientName + "ApiClientOptions")
                    );
            }


            //List<Type> apiClientClasses = finder.GetApiClientClasses();
            ////var apiClient = Activator.CreateInstance(apiClientClass, apiClientOptions) as BaseApiClient;

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
                    //apiClientClasses.ForEach(type => services.AddSingleton(typeof(BaseApiClient), type));
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
