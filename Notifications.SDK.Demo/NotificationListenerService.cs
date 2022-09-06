using Maranics.Notifications.SDK;
using Maranics.Notifications.SDK.Models;
using Notifications.SDK.Demo;

public class NotificationListenerService : IHostedService
{
    private readonly NotificationsListener _notificationListener;
    private readonly ILogger<NotificationListenerService> _logger;

    public NotificationListenerService(NotificationsListener notificationListener, ILogger<NotificationListenerService> logger)
    {
        _notificationListener = notificationListener;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Notification Listener Service is starting.");

        await _notificationListener.Subscribe(new SubscribeToNotificationsRequest
        {
            ApplicationName = Constants.AppName
        },
        ProcessNewNotification, ProcessSubscription, cancellationToken);

        _logger.LogInformation("Notification Listener Service is running.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Notification Listener Service is stopping.");

        return Task.CompletedTask;
    }

    private Task ProcessNewNotification(ReceiveNotificationMessage notificationMessage)
    {
        _logger.LogInformation(@$"New notification received from notification service:
            Notification Id: {notificationMessage.NotificationId}
            Notification created on: {notificationMessage.CreatedOn}
            Notification content: {notificationMessage.Content}");

        return Task.CompletedTask;
    }


    private Task ProcessSubscription(SubscribeToNotificationsResponse subscription)
    {
        if (!string.IsNullOrEmpty(subscription.Result?.ApplicationName))
        {
            foreach (var tenant in subscription.Result.TenantNames ?? Enumerable.Empty<string>())
            {
                _logger.LogInformation($"Application '{Constants.AppName}':  Tenant {tenant} is connected to listen notifications.");
            }
        }
        else
        {
            _logger.LogWarning($"Application '{Constants.AppName}': Failed to subscribe - {subscription.Error?.Message}");
        }

        return Task.CompletedTask;
    }
}

