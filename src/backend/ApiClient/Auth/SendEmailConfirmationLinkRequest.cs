namespace ApiClient.Auth;

public record SendEmailConfirmationLinkRequest(
    string Email,
    string? RedirectUrl);