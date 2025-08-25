namespace ITC.Authorization.Services.Mq;

public interface IMqHandler<TPayload>
{
    Task Handle<T>(T payload, CancellationToken cancellation);
}