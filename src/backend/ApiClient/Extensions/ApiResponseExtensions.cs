using Refit;

namespace ApiClient.Extensions;

public static class ApiResponseExtensions
{
    public static async Task<ProblemDetails?> GetErrorAsync(this IApiResponse response)
    {
        if (response.IsSuccessful)
            throw new InvalidOperationException("Response must have unsuccessful status");
        
        return await response.Error.GetContentAsAsync<ProblemDetails>();
    }
}