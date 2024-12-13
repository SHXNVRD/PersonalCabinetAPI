using Application.Cards.Commands.Deactivate;
using Riok.Mapperly.Abstractions;

namespace API.DTOs.CardController;

public record DeactivateCardRequest(int CardNumber);

[Mapper]
public static partial class DeactivateCardMapper
{
    public static partial DeactivateCardCommand ToCommand(DeactivateCardRequest request);
}