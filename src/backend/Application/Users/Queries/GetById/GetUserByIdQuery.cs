using Application.Cards.DTOs;
using FluentResults;
using MediatR;

namespace Application.Users.Queries.GetById;

public class GetUserByIdQuery : IRequest<Result<GetUserByIdResponse>>
{
    public Guid Id { get; set; }
}

public record GetUserByIdResponse(
    Guid Id,
    string UserName,
    string FirstName,
    string LastName,
    string? Patronymic,
    DateOnly? DayOfBirth,
    string PhoneNumber,
    bool PhoneNumberConfirmed,
    bool TwoFactorEnabled,
    string Email,
    CardResponse[] Cards);