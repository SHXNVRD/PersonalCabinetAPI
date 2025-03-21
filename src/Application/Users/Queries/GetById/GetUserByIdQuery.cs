using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Users.DTOs;
using FluentResults;
using MediatR;

namespace Application.Users.Queries.GetById;

public class GetUserByIdQuery : IRequest<Result<GetUserByIdResponse>>
{
    public string Id { get; set; }
}

public record GetUserByIdResponse(
    Guid Id,
    string Name,
    DateOnly? DayOfBirth,
    string PhoneNumber,
    bool PhoneNumberConfirmed,
    bool TwoFactorEnabled,
    string Email);