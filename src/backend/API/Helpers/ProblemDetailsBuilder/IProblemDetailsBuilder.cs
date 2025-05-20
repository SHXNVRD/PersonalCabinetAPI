using Microsoft.AspNetCore.Mvc;

namespace API.Helpers.ProblemDetailsBuilder;

public interface IProblemDetailsBuilder
{
    IProblemDetailsBuilder AddTitle(string? title = null);
    IProblemDetailsBuilder AddDetail(string? detail = null);
    IProblemDetailsBuilder AddStatus();
    IProblemDetailsBuilder AddType(string? type = null);
    IProblemDetailsBuilder AddInstance(string instance);
    IProblemDetailsBuilder AddExtension(string key, object value);
    ProblemDetails Build();
}