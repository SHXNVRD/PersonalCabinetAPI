using FluentResults;
using MediatR;

namespace Application.Cards.Commands.Block;

public class BlockCardCommand : IRequest<Result>
{
    public string UserId { get; set; }
    public string Number { get; set; }
}