using Application.Cards.Commands.Block;
using Riok.Mapperly.Abstractions;

namespace API.Controllers.Card.DTOs;

public record BlockCardRequest(int CardNumber);

[Mapper]
public static partial class DeactivateCardMapper
{
    public static partial BlockCardCommand ToCommand(BlockCardRequest request);
}