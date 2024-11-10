using Api.Dtos.Users;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Tickets;
using Application.Users.Commands;
using Domain.Identity;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("users")] 
[ApiController]
public class UserController(ISender sender, IUserQueries userQueries) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("getAll/")]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await userQueries.GetAll(cancellationToken);

        return entities.Select(UserDto.FromDomainModel).ToList();
    }

    [AllowAnonymous]
    [HttpGet("getById/{userId:guid}")]
    public async Task<ActionResult<UserDto>> Get([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var entity = await userQueries.GetById(new UserId(userId), cancellationToken);

        return entity.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            () => NotFound());
    }
    
    [AllowAnonymous]
    [HttpGet("getBalanceById/{userId:guid}")]
    public async Task<ActionResult<UserBalanceDto>> GetBalanceById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var entity = await userQueries.GetById(new UserId(userId), cancellationToken);

        return entity.Match<ActionResult<UserBalanceDto>>(
            u => UserBalanceDto.FromDomainModel(u),
            () => NotFound());
    }

    [AllowAnonymous]
    [HttpPost("create/")]
    public async Task<ActionResult<UserDto>> Create([FromBody] UserCreateDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateUserCommand
        {
            Login = request.Login,
            Password = request.Password,
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }

    
    [Authorize]
    [RequiresClaim(IdentityData.IsAdminClaimName, "True")]
    [HttpPut("update/{userId:guid}")]
    public async Task<ActionResult<UserDto>> Update([FromRoute] Guid userId, [FromBody] UserUpdateDto request,
        CancellationToken cancellationToken)
    {

        var input = new UpdateUserCommand
        {
            UserId = userId,
            Login = request.Login,
            Password = request.Password,
            Balance = request.Balance,
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<UserDto>>(
            user => UserDto.FromDomainModel(user),
            e => e.ToObjectResult());
    }

    [Authorize]
    [HttpPut("AddToBalance/{userId:guid}")]
    public async Task<ActionResult<UserDto>> Update([FromRoute] Guid userId, [FromBody] UserUpdateBalanceDto request,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userid");
        var userIdFromToken = new Guid(userIdClaim!.Value);
        
        var input = new UpdateUserBalanceCommand
        {
            Balance = request.Balance,
            UserId = userId,
            UserIdFromToken = userIdFromToken
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<UserDto>>(
            user => UserDto.FromDomainModel(user),
            e => e.ToObjectResult());
    }
    
    [Authorize]
    [RequiresClaim(IdentityData.IsAdminClaimName, "True")]
    [HttpDelete("delete/{userId:guid}")]
    public async Task<ActionResult<UserDto>> Delete([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        
        var input = new DeleteUserCommand
        {
            UserId = userId,
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }
}