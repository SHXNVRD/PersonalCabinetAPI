using Application.Cards.Commands.Activate;
using Riok.Mapperly.Abstractions;

namespace API.DTOs.CardController;
public record ActivateCardRequest(int CardNumber, string CardCode);

[Mapper]
public static partial class ActivateCardMapper
{
    [MapperIgnoreTarget(nameof(ActivateCardCommand.UserId))]
    public static partial ActivateCardCommand ToCommand(ActivateCardRequest request);
}