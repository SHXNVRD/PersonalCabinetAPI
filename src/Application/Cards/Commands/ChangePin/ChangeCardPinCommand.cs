using FluentResults;
using MediatR;

namespace Application.Cards.Commands.ChangePin;

public class ChangeCardPinCommand : IRequest<Result>
{
    public string Pin { get; set; }
    public Guid CardId { get; set; }
}