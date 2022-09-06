using Maranics.AppStore.SDK;
using Maranics.AppStore.SDK.Interfaces;
using Maranics.Notifications.SDK;
using Notifications.SDK.Demo;

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json", false, true).AddEnvironmentVariables().Build();

using IHost host = Host.CreateDefaultBuilder()
    .ConfigureServices((_, services) =>
    {
        services.ConfigureAppStore(configuration);
        services.AddNotifications(configuration, serviceProvider =>
        {
            var tokenProvider = serviceProvider.GetService<IAccessTokenProvider>();
            return tokenProvider!.GetTokenAsync();
        });
        services.AddHostedService<NotificationListenerService>();
        services.AddHostedService<NotificationSenderService>();
    })
    .ConfigureLogging((context, config)
        => config.AddConfiguration(configuration.GetSection("Logging")))
    .Build();

host.Run();
Console.Read();