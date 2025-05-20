using Application.Interfaces;
using Org.BouncyCastle.Asn1.Ocsp;

namespace API.Services;

public class LinkService : ILinkService
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LinkService(
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor)
    {
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetUriByAction(string? action = default, string? controller = default, 
        object? values = default, string? scheme = default)
    {
        return _linkGenerator.GetUriByAction(_httpContextAccessor.HttpContext, action, controller, values, scheme);
    }
}