using FluentResults;
using MediatR;

namespace Application.Cards.Commands.Block;

public class BlockCardCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public Guid CardId { get; set; }
}