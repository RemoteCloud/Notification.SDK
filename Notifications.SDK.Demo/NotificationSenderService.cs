using Maranics.Notifications.SDK;

namespace Notifications.SDK.Demo
{
    public class NotificationSenderService : BackgroundService
    {
        private readonly INotificationsManager _notificationsManager;
        private readonly ILogger<NotificationSenderService> _logger;

        public NotificationSenderService(INotificationsManager notificationsManager, ILogger<NotificationSenderService> logger)
        {
            _notificationsManager = notificationsManager;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            const string tenant = "Development";
            Notification notification;
            int currentMessageIndex = 1;
            var request = new DispatchNotificationRequest()
            {
                Source = new NotificationSource()
                {
                    SenderId = Guid.NewGuid().ToString(),
                    SenderName = Constants.AppName
                },
                Content = new NotificationContent()
                {
                    Type = NotificationType.Information,
                },
                RecipientFilters = new List<NotificationRecipientFilter>()
                {
                     new NotificationRecipientFilter(){ Scope = Constants.AppName, LocationId = Guid.NewGuid(), Users = new List<Guid>(){ Guid.NewGuid() } }
                }
            };

            while (cancellationToken.IsCancellationRequested == false)
            {
                request.Content.Body = $"Message #{currentMessageIndex}";
                DispatchNotificationResponse response = await _notificationsManager.DispatchNotificationAsync(tenant, body: request);
                foreach (var notificationId in response.NotificationIds)
                {
                    notification = _notificationsManager.GetNotification(notificationId, tenant);
                    _logger.LogInformation($"Notification (id: {notification.NotificationId}) sent to user {notification.UserId}.");
                }
                currentMessageIndex++;
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}
