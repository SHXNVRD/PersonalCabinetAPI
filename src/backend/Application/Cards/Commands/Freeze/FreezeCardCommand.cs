using FluentResults;
using MediatR;

namespace Application.Cards.Commands.Freeze;

public class FreezeCardCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public Guid CardId { get; set; }
}
