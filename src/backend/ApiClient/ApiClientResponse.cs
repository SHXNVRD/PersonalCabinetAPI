    using System.Net;
    using System.Text.Json;

    namespace ApiClient;

    public class ApiClientResponse(HttpStatusCode code, string content)
    {
        public HttpStatusCode Code { get; } = code;
        
        public bool IsSuccessStatusCode => (int)Code is >= 200 and <= 299;
        
        public string StringContent { get; } = content;

        public ProblemDetails? GetError()
        {
            if (IsSuccessStatusCode)
                throw new InvalidOperationException("Response must have unsuccessful status code");

            return JsonSerializer.Deserialize<ProblemDetails>(StringContent);
        }
    }

    public class ApiClientResponse<T>(HttpStatusCode code, string content) : ApiClientResponse(code, content)
    {
        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
        
        public T? Content => DeserializeContent();

        private T? DeserializeContent()
        {
            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(StringContent, typeof(T));
            }

            return JsonSerializer.Deserialize<T>(StringContent, _serializerOptions);
        }
    }