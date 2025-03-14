using Application.Cards.Commands.Block;
using Riok.Mapperly.Abstractions;

namespace API.DTOs.CardController;

public record DeactivateCardRequest(int CardNumber);

[Mapper]
public static partial class DeactivateCardMapper
{
    public static partial BlockCardCommand ToCommand(DeactivateCardRequest request);
}