﻿using FluentResults;
using MediatR;

namespace Application.Users.Commands.ResetPassword;

public class ResetPasswordCommand : IRequest<Result>
{
    public string Email { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
}