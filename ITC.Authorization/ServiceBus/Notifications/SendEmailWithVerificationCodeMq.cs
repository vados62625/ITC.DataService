namespace ITC.Authorization.ServiceBus.Notifications;

public class SendEmailWithVerificationCodeMq
{
	public required string Email { get; set; }
	public required string VerificationCode { get; set; }
}