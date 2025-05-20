using Application.Users.Commands.EmailConfirmation;
using Microsoft.AspNetCore.Mvc;
using Riok.Mapperly.Abstractions;

namespace API.Controllers.Authentication.DTOs;

public record ConfirmEmailRequest(
    [FromQuery(Name = "email")] string Email,
    [FromQuery(Name = "token")] string Token,
    [FromQuery(Name = "redirectUrl")] string? RedirectUrl);

[Mapper]
public partial class ConfirmEmailMapper
{
    [MapperIgnoreSource("RedirectUrl")]
    public static partial EmailConfirmationCommand ToCommand(ConfirmEmailRequest request);
}