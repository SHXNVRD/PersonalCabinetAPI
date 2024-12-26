using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/v1")]
[ApiController]
public class PurchaseController : ControllerBase
{
    [HttpPost("purchases")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult New()
    {
        return Ok();
    }
}