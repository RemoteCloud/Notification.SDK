using Maranics.AppStore.SDK;
using Maranics.AppStore.SDK.Interfaces;
using Maranics.Notifications.SDK;
using Microsoft.AspNetCore.Mvc;
using Notifications.SDK.Demo;

var builder = WebApplication.CreateBuilder();
builder.Configuration.AddJsonFile("appsettings.Development.json", false, true).AddEnvironmentVariables();
builder.Services.ConfigureAppStore(builder.Configuration);
builder.Services.AddNotifications(builder.Configuration, serviceProvider =>
{
    var tokenProvider = serviceProvider.GetService<IAccessTokenProvider>();
    return tokenProvider!.GetTokenAsync();
});
builder.Services.AddHostedService<NotificationListenerService>();
builder.Services.AddHostedService<NotificationSenderService>();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
const string tenant = "Development";
app.Urls.Add("http://localhost:3000");
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/subscriptions/vapid", ([FromServices] IPushNotificationSubscriptionsManager pushNotificationSubscriptionsManager) => { return pushNotificationSubscriptionsManager.GetVapidPublicKey(tenant); });
app.MapPost("/subscriptions", (Subscription subscription, [FromServices] IPushNotificationSubscriptionsManager pushNotificationSubscriptionsManager) =>
{
    var pushNotificationSubscription = new PushNotificationSubscription()
    {
        Endpoint = subscription.Endpoint,
        PublicKey = subscription.PublicKey,
        AuthKey = subscription.AuthKey,
        ApplicationServerKey = subscription.Vapid,
        Scope = subscription.AppName,
        UserId = subscription.UserId
    };
    return pushNotificationSubscriptionsManager.RegisterPushNotificationSubscription(tenant, body: pushNotificationSubscription);
});

app.Run();