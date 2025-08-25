namespace ITC.Authorization.ServiceBus.Notifications;

public class SendPlatformNotificationCommand
{
    public required string Message { get; set; }
    public bool IsHtml { get; set; }
}