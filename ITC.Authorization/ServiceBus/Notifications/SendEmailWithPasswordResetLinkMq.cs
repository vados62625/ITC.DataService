namespace ITC.Authorization.ServiceBus.Notifications;

public class SendEmailWithPasswordResetLinkMq
{
    public required string Email { get; set; }
    public required string PasswordResetTokenLink { get; set; }
}