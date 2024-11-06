using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Tickets.Exceptions;
using Domain.Identity;
using Domain.Users;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Tickets.Commands;

public record CreateTokenCommand : IRequest<Result<string, TokenException>>
{
    public required string Login { get; init; }
    public required string Password { get; init; }
}

public class CreateTokenCommandHandler(IUserRepository userRepository, IConfiguration configuration)
    : IRequestHandler<CreateTokenCommand, Result<string, TokenException>>
{
    private IConfiguration Config { get; } = configuration; 

    private const string TokenSecret = "ForTheLoveOfGodStoreAndLoadThisSecurely";
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(8);
    
    public async Task<Result<string, TokenException>> Handle(CreateTokenCommand request,
        CancellationToken cancellationToken)
    {
        
        var user = await userRepository.GetByLoginAndPassword(request.Login, request.Password, cancellationToken);

         return await user.Match(
             u => GenerateToken(u),
             () => Task.FromResult<Result<string, TokenException>>(new LoginException())
         );
    }

    private async Task<Result<string, TokenException>> GenerateToken(User u)
    {
        try
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(TokenSecret);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, u.Login),
                new("userid", u.Id.ToString()),
                new(IdentityData.IsAdminClaimName, u.IsAdmin.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TokenLifetime),
                Issuer = Config["JwtSettings:Issuer"],
                Audience = Config["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);
            return await Task.FromResult<Result<string, TokenException>>(jwt);
            
        }
        catch (Exception e)
        {
            return new TicketUnknownException(u.Login, e);
        }
    }
    
}