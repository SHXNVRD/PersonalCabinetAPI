namespace API.Requests;

public record class ResetPasswordRequest(
    string Email,
    string Token,
    string NewPassword);