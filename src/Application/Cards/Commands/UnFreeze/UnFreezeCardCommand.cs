using FluentResults;
using MediatR;

namespace Application.Cards.Commands.UnFreeze;

public class UnFreezeCardCommand : IRequest<Result>
{
    public string UserId { get; set; }
    public Guid CardId { get; set; }
}