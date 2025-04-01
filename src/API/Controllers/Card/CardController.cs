using System.Security.Claims;
using API.Controllers.Card.DTOs;
using API.Extensions;
using Application.Cards.Commands.Activate;
using Application.Cards.Commands.Block;
using Application.Cards.Commands.ChangePin;
using Domain.Aggregates.Base;
using Domain.Shared.Errors;
using FluentResults;
using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ActivateCardMapper = API.Controllers.Card.DTOs.ActivateCardMapper;
using DeactivateCardMapper = API.Controllers.Card.DTOs.DeactivateCardMapper;

namespace API.Controllers.Card;

[Route("api/v1/cards")]
[ApiController]
public class CardController : ControllerBase
{
    private readonly IMediator _mediatR;

    public CardController(IMediator mediatR)
    {
        _mediatR = mediatR;
    }

    [HttpPut("activation")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ActivateCardResponse>> ActivateCardByNumber([FromBody] ActivateCardRequest request)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Result
                .Fail(new Conflict("Access token does not contain user id"))
                .ToObjectResult(HttpContext);

        var command = ActivateCardMapper.ToCommand(request);
        command.UserId = userId;
            
        var result = await _mediatR.Send(command);

        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);

        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/block")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> BlockCardById([FromRoute] Guid id)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Result
                .Fail(new Conflict("Access token does not contain user id"))
                .ToObjectResult(HttpContext);

        BlockCardCommand command = new()
        {
            CardId = id,
            UserId = userId
        };
        
        var result = await _mediatR.Send(command);

        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);

        return NoContent();
    }

    [HttpGet("{id:guid}/operations")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetOperationsById([FromRoute] Guid id, [FromQuery] int page, [FromQuery] int pageSize)
    {
        var request = new GetOperationsByIdRequest(id, page, pageSize);
        var command = GetOperationsByIdMapper.ToQuery(request);
        var result = await _mediatR.Send(command);
        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);

        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/pin")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> ChangePin([FromRoute] Guid id, [FromBody] ChangeCardPinRequest request)
    {
        if (id == Guid.Empty)
            return Result.Fail(new InvalidData("Id is required")).ToObjectResult(HttpContext);

        var command = new ChangeCardPinCommand
        {
            CardId = id,
            Pin = request.Pin
        };

        var result = await _mediatR.Send(command);
        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);

        return NoContent();
    }
}