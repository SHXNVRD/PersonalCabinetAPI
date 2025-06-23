using FluentResults;
using MediatR;

namespace Application.Cards.Commands.UnFreeze;

public class UnFreezeCardCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public Guid CardId { get; set; }
}