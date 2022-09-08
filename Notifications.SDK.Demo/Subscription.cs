using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Notifications.SDK.Demo
{
    public class Subscription
    {
        public string Endpoint { get; set; }
        public string PublicKey { get; set; }

        public string AuthKey { get; set; }

        public string AppName { get; set; }

        public string Vapid { get; set; }

        public Guid UserId { get; set; }
    }
}
