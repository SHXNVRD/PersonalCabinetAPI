using System.Text.Json.Serialization;

namespace ApiClient;

public record ProblemDetails(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("status")] int Status,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("traceId")] string TraceId,
    [property: JsonPropertyName("errors")] string[] Errors);
