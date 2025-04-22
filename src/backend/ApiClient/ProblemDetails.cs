namespace ApiClient;

public record ProblemDetails(
    string Type,
    string Status,
    string Title,
    string TraceId,
    string[] Errors);
