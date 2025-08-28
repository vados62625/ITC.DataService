using ITC.ReportService.Hub;
using Microsoft.AspNetCore.SignalR;

namespace ITC.ReportService.Services;

public interface ISignalRService
{
    Task SendMessageToAllAsync<T>(string methodName, T message);
    Task SendMessageToGroupAsync<T>(string groupName, string methodName, T message);
    Task SendMessageToUserAsync<T>(string userId, string methodName, T message);
}

public class SignalRService : ISignalRService
{
    private readonly IHubContext<EngineHub> _hubContext;

    public SignalRService(IHubContext<EngineHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendMessageToAllAsync<T>(string methodName, T message)
    {
        await _hubContext.Clients.All.SendAsync(methodName, message);
    }

    public async Task SendMessageToGroupAsync<T>(string groupName, string methodName, T message)
    {
        await _hubContext.Clients.Group(groupName).SendAsync(methodName, message);
    }

    public async Task SendMessageToUserAsync<T>(string userId, string methodName, T message)
    {
        await _hubContext.Clients.User(userId).SendAsync(methodName, message);
    }
}