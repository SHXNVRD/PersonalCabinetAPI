using System.Net.Http.Json;
using System.Text.Json;

namespace ApiClient;

public class ApiClientExecutor
{
    protected virtual string ApiPrefix => "api";
    private readonly GasStationClient _apiClient;

    public ApiClientExecutor(GasStationClient client)
    {
        _apiClient = client;
    }

    protected async Task<ApiClientResponse<T>> GetAsync<T>(string uri, CancellationToken token = default)
    {
        return await SendWithBody<T>(HttpMethod.Get, uri, token: token);
    }

    protected async Task<ApiClientResponse<T>> PostAsync<T>(string uri, object? body = null, CancellationToken token = default)
    {
        return await SendWithBody<T>(HttpMethod.Post, uri, body, token);
    }

    protected async Task<ApiClientResponse> PostAsync(string uri, object? body = null, CancellationToken token = default)
    {
        return await SendWithBody<string>(HttpMethod.Post, uri, body, token);
    }

    protected async Task<ApiClientResponse> PutAsync(string uri, object? body = null, CancellationToken token = default)
    {
        return await SendWithBody<string>(HttpMethod.Put, uri, body, token);
    }

    protected async Task<ApiClientResponse<T>> PatchAsync<T>(string uri, object? body = null, CancellationToken token = default)
    {
        return await SendWithBody<T>(HttpMethod.Patch, uri, body, token);
    }

    protected async Task<ApiClientResponse> DeleteAsync(string uri, CancellationToken token = default)
    {
        return await SendWithBody<string>(HttpMethod.Delete, uri, token: token);
    }

    private async Task<ApiClientResponse<T>> SendWithBody<T>(HttpMethod method, string uri, object? body = null, CancellationToken token = default)
    {
        using var requestMessage = new HttpRequestMessage(method, $"{ApiPrefix}{uri}");

        _apiClient.Log($"method: {method}");
        _apiClient.Log($"url: {ApiPrefix}{uri}");

        if (body != null)
        {
            requestMessage.Content = JsonContent.Create(body);
            _apiClient.Log($"body: {JsonSerializer.Serialize(body)}");
        }

        var response = await _apiClient.HttpClient.SendAsync(requestMessage, token);
        return ProcessResponse<T>(response, token);
    }

    private ApiClientResponse<T> ProcessResponse<T>(HttpResponseMessage response, CancellationToken token = default)
    {
        using var responseStreamReader = new StreamReader(response.Content.ReadAsStream(token));

        var responseContent = responseStreamReader.ReadToEnd();
        _apiClient.Log("response: " + responseContent);
        return new ApiClientResponse<T>(response.StatusCode, responseContent);
    }
}