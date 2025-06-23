using API.Controllers.Product.DTOs;
using API.Extensions;
using Application.Products.Commands.DeleteProduct;
using Application.Refunds.Commands;
using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Product;

[Route("api/products")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<CreateRefundResponse>> Create([FromBody] CreateProductRequest request)
    {
        var command = CreateProductMapper.ToCommand(request);
        
        var result = await _mediator.Send(command);
        
        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);

        return result.ToActionResult();
    }
    
    [HttpDelete("{id:long:min(0)}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<CreateRefundResponse>> Delete([FromRoute] long id)
    {
        var command = new DeleteProductCommand
        {
            Id = id
        };
        
        var result = await _mediator.Send(command);
        
        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);

        return NoContent();
    }
    
    [HttpPut("{id:long:min(0)}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<CreateRefundResponse>> Delete([FromRoute] long id, [FromBody] UpdateProductRequest request)
    {
        var command = UpdateProductMapper.ToCommand(request);
        command.Id = id;
        
        var result = await _mediator.Send(command);
        
        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);

        return NoContent();
    }
}