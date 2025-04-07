using FluentResults;
using MediatR;

namespace Application.Cards.Commands.Freeze;

public class FreezeCardCommand : IRequest<Result>
{
    public string UserId { get; set; }
    public Guid CardId { get; set; }
}
