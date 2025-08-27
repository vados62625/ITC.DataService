using ITC.Domain.Dto;
using ITC.ReportService.Extensions;
using Microsoft.Net.Http.Headers;

namespace ITC.ReportService.Services;

public interface IDataServiceClient
{
    Task<bool> UploadCsv(IFormFile file);
}

public class DataServiceClient : IDataServiceClient
{
    private readonly HttpClient _client;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DataServiceClient(HttpClient client, IHttpContextAccessor httpContextAccessor)
    {
        _client = client;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<bool> UploadCsv(IFormFile file)
    {
        var uri = "/Csv/Upload";
        var response = await _client.ApiPost(content =>
        {
            content.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);
        }, uri);
        return response.IsSuccessStatusCode;
    }

    private string GetJwtFromContext()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            throw new Exception("JWT not found");
        return token;
    }
}