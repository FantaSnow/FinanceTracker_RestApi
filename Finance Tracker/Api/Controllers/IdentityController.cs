using Api.Dtos;
using Api.Modules.Errors;
using Application.Tickets.Commands;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace Api.Controllers;

[ApiController]
[Route("identity")]
public class IdentityController(ISender sender) : ControllerBase
{

    [HttpPost("token")]
    public async Task<ActionResult<string>> GenerateToken([FromBody] TokenGenerationRequest request, CancellationToken cancellationToken)
    {
        var input = new CreateTokenCommand
        {
            Login = request.Login,
            Password = request.Password
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<string>>(
            jwt => Ok(jwt), 
            e => e.ToObjectResult());
    }
}

