namespace Application.Interfaces;

public interface ILinkService
{
    string? GetUriByAction(string? action = default, string? controller = default, 
        object? values = default, string? scheme = default);
}