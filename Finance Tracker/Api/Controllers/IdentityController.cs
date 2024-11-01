using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Dtos;
using Api.Identity;
using Api.Modules;
using Application.Common.Interfaces.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Domain.Users;

namespace Api.Controllers;

[ApiController]
[Route("identity")]
public class IdentityController : ControllerBase
{
    private const string TokenSecret = "ForTheLoveOfGodStoreAndLoadThisSecurely";
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(8);

    private readonly IConfiguration _config;
    private readonly IUserQueries _userQueries;

    public IdentityController(IUserQueries userQueries, IConfiguration config)
    {
        _userQueries = userQueries;
        _config = config;
    }

    [HttpPost("token")]
    public async Task<ActionResult<string>> GenerateToken([FromBody] TokenGenerationRequest request, CancellationToken cancellationToken)
    {
        var user = await _userQueries.GetByLoginQ(request.Login, cancellationToken);
        
        if (user == null || user.Password != request.Password)
            return Unauthorized("Invalid login or password");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(TokenSecret);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Login),
            new(JwtRegisteredClaimNames.Email, user.Login),
            new("userid", user.Id.ToString()),
            new(IdentityData.IsAdminClaimName, user.IsAdmin.ToString()) 
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(TokenLifetime),
            Issuer = _config["JwtSettings:Issuer"],
            Audience = _config["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);

        return Ok(jwt);
    }
}

