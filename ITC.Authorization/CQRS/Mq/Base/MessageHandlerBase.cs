using System.Text.Json;
using Amazon.SQS.Model;
using ITC.Authorization.Exceptions;
using MediatR;
using NLog;
using ILogger = NLog.ILogger;

namespace ITC.Authorization.CQRS.Mq.Base;

public class MqMessageBase<TPayload> : IRequest
{
    public Message Message { get; }

    public MqMessageBase(Message message)
    {
        Message = message;
    }

    public TPayload GetPayload()
    {
        return TryDeserialize<TPayload>(Message);
    }

    private T TryDeserialize<T>(Message message)
    {
        var payload = JsonSerializer.Deserialize<T>(message.Body);
        if (payload != null)
            return payload;
        throw new WrongPayloadException(typeof(T).Name);
    }
}

public abstract class MessageHandlerBase<TPayload> : IRequestHandler<MqMessageBase<TPayload>>
{
    protected readonly ILogger Log;

    protected MessageHandlerBase()
    {
        Log = LogManager.GetLogger($"MqHandler_{typeof(TPayload).Name}");
    }
    public async Task Handle(MqMessageBase<TPayload> request, CancellationToken cancellationToken)
    {
        try
        {
            var payload = request.GetPayload();
            await Handle(payload, cancellationToken);
        }
        catch (WrongPayloadException e)
        {
            Log.Error(e);
        }
        catch (Exception e)
        {
            Log.Error(e);
            throw;
        }
    }

    protected abstract Task Handle(TPayload message, CancellationToken cancellationToken);
}