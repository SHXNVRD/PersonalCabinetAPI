using Application.Cards.Commands.Activate;
using Riok.Mapperly.Abstractions;

namespace API.Controllers.Card.DTOs;
public record ActivateCardRequest(string CardNumber, string CardPinCode);

[Mapper]
public static partial class ActivateCardMapper
{
    [MapperIgnoreTarget(nameof(ActivateCardCommand.UserId))]
    public static partial ActivateCardCommand ToCommand(ActivateCardRequest request);
}