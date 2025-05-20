using System.Security.Claims;
using API.Controllers.Card.DTOs;
using API.Extensions;
using Application.Cards.Commands.Activate;
using Application.Cards.Commands.Block;
using Application.Cards.Commands.ChangePin;
using Application.Cards.Commands.Freeze;
using Application.Cards.Commands.UnFreeze;
using Domain.Aggregates.Base;
using Domain.Shared.Errors;
using FluentResults;
using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;
using ActivateCardMapper = API.Controllers.Card.DTOs.ActivateCardMapper;
using DeactivateCardMapper = API.Controllers.Card.DTOs.DeactivateCardMapper;

namespace API.Controllers.Card;

[Route("api/cards")]
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ActivateCardResponse>> ActivateByNumber([FromBody] ActivateCardRequest request)
    {
        var userIdResult = HttpContext.User.GetUserId();
        if (userIdResult.IsFailed)
            return userIdResult.ToObjectResult(HttpContext);

        var command = ActivateCardMapper.ToCommand(request);
        command.UserId = userIdResult.Value;
            
        var result = await _mediatR.Send(command);

        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);

        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/block")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> BlockById([FromRoute] Guid id)
    {
        var userIdResult = HttpContext.User.GetUserId();
        if (userIdResult.IsFailed)
            return userIdResult.ToObjectResult(HttpContext);

        if (!Guid.TryParse(userIdResult.Value, out var userId))
            throw new Exception($"Access token contains invalid id: {userIdResult.Value}");
                
        var command = new BlockCardCommand
        {
            UserId = userId,
            CardId = id
        };
        
        var result = await _mediatR.Send(command);

        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);

        return NoContent();
    }

    [HttpPut("{id:guid}/freezing")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> FreezeById([FromRoute] Guid id)
    {
        var userIdResult = HttpContext.User.GetUserId();
        if (userIdResult.IsFailed)
            return userIdResult.ToObjectResult(HttpContext);
        
        if (!Guid.TryParse(userIdResult.Value, out var userId))
            throw new Exception($"Access token contains invalid id: {userIdResult.Value}");
        
        var command = new FreezeCardCommand
        {
            UserId = userId,
            CardId = id
        };

        var result = await _mediatR.Send(command);

        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);

        return NoContent();
    }
    
    [HttpPut("{id:guid}/unfreezing")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> UnFreezeById([FromRoute] Guid id)
    {
        var userIdResult = HttpContext.User.GetUserId();
        if (userIdResult.IsFailed)
            return userIdResult.ToObjectResult(HttpContext);
        
        if (!Guid.TryParse(userIdResult.Value, out var userId))
            throw new Exception($"Access token contains invalid id: {userIdResult.Value}");
        
        var command = new UnFreezeCardCommand
        {
            UserId = userId,
            CardId = id
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
        var validator = new GetOperationsByIdRequestValidator();
        
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.ToValidationProblemErrors());
            
        var command = GetOperationsByIdMapper.ToQuery(request);
        
        var result = await _mediatR.Send(command);
        
        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);

        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/pin")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> ChangePin([FromRoute] Guid id, [FromBody] ChangeCardPinRequest request)
    {
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