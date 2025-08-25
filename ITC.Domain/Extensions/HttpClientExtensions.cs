using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ITC.Domain.Extensions;

public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        IgnoreReadOnlyFields = false,
        IgnoreReadOnlyProperties = false,
        PropertyNameCaseInsensitive = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new JsonStringEnumConverter() }
    };

    private static readonly Random Rnd = new();

    public static async Task<HttpResponseMessage> ApiPost(this HttpClient client, object content, string uri,
        string jwt = "", string requestId = "")
    {
        var httpreq = CreateRequest(HttpMethod.Post, uri, jwt, requestId);
        var json = JsonSerializer.Serialize(content);
        httpreq.Content = new StringContent(json, Encoding.UTF8, "application/json");
        return await client.SendAsync(httpreq);
    }
    
    public static async Task<HttpResponseMessage> ApiPost(this HttpClient client, Action<MultipartFormDataContent> configureContent, 
        string uri, string jwt = "", string requestId = "")
    {
        var httpreq = CreateRequest(HttpMethod.Post, uri, jwt, requestId);
        var content = new MultipartFormDataContent();
        configureContent(content);
        httpreq.Content = content;
        return await client.SendAsync(httpreq);
    }

    public static async Task<HttpResponseMessage> ApiPost(this HttpClient client, string uri,
        string jwt = "", string requestId = "")
    {
        var httpreq = CreateRequest(HttpMethod.Post, uri, jwt, requestId);
        return await client.SendAsync(httpreq);
    }

    public static async Task<HttpResponseMessage> ApiPut(this HttpClient client, object content, string uri,
        string jwt = "", string requestId = "")
    {
        var httpreq = CreateRequest(HttpMethod.Put, uri, jwt, requestId);
        var json = JsonSerializer.Serialize(content);
        httpreq.Content = new StringContent(json, Encoding.UTF8, "application/json");
        return await client.SendAsync(httpreq);
    }
    public static async Task<HttpResponseMessage> ApiPatch(this HttpClient client, object content, string uri,
        string jwt = "", string requestId = "")
    {
        var httpreq = CreateRequest(HttpMethod.Patch, uri, jwt, requestId);
        var json = JsonSerializer.Serialize(content);
        httpreq.Content = new StringContent(json, Encoding.UTF8, "application/json");
        return await client.SendAsync(httpreq);
    }
    public static async Task<HttpResponseMessage> ApiDelete(this HttpClient client, string uri,
        string jwt = "", string requestId = "")
    {
        var httpreq = CreateRequest(HttpMethod.Delete, uri, jwt, requestId);
        return await client.SendAsync(httpreq);
    }
    public static async Task<HttpResponseMessage> ApiDelete(this HttpClient client, object content, string uri,
        string jwt = "", string requestId = "")
    {
        var httpreq = CreateRequest(HttpMethod.Delete, uri, jwt, requestId);
        var json = JsonSerializer.Serialize(content);
        httpreq.Content = new StringContent(json, Encoding.UTF8, "application/json");
        return await client.SendAsync(httpreq);
    }
    public static async Task<HttpResponseMessage> ApiGet(this HttpClient client, string uri,
        string jwt = "", string requestId = "")
    {
        var httpreq = CreateRequest(HttpMethod.Get, uri, jwt, requestId);
        return await client.SendAsync(httpreq, HttpCompletionOption.ResponseHeadersRead);
    }

    public static async Task<T> ReadAs<T>(this HttpContent responseContent, JsonSerializerOptions? options = null)
    {
        options ??= DefaultOptions;
        await using var jsonStream = await responseContent.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<T>(jsonStream, options);
        return result == null ? throw new JsonException($"Cannot deserialize content to type {typeof(T).Name}") : result;
    }

    public static HttpRequestMessage CreateRequest(HttpMethod method, string uri, string? jwt, string? requestId)
    {
        var httpreq = new HttpRequestMessage(method, uri);
        FillHeaders(httpreq.Headers, jwt, requestId);
        return httpreq;
    }
    public static async Task<HttpResponseMessage> PostFile(this HttpClient client, string fileName, string contentType, Stream readStresm, string uri, string jwt = "", string requestId = "")
    {

        using var content = new MultipartFormDataContent();
        //var fileContent = new StreamContent(file.OpenReadStream(500 * 1024 * 1024));
        var fileContent = new StreamContent(readStresm);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        content.Add(fileContent, "\"file\"", fileName);

        //FillHeaders(content.Headers, jwt, requestId);
        var result = await client.PostAsync(uri, content);

        Console.WriteLine($"{uri} return {result.StatusCode}");

        return result;
    }
    public static void FillHeaders(HttpHeaders headers, string? jwt, string? requestId)
    {
        if (!string.IsNullOrEmpty(jwt))
        {
            headers.Add("Authorization", "Bearer " + jwt);
        }

        if (string.IsNullOrEmpty(requestId))
        {
            requestId = GenerateRequestId();
        }

        headers.Add("Request-id", requestId);
    }

    public static string GenerateRequestId()
    {
        var rndBuff = new byte[8];
        Rnd.NextBytes(rndBuff);
        return Convert.ToBase64String(rndBuff);
    }


}