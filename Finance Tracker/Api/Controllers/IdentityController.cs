using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Dtos;
using Api.Dtos.Statistics;
using Api.Modules;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Tickets.Commands;
using Domain.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Domain.Users;
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

