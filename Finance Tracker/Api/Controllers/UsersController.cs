using Api.Dtos.Users;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Users.Commands;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("users")]
[ApiController]
public class UsersController(ISender sender, IUserQueries userQueries) : ControllerBase
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

    [Authorize]
    [HttpPost("create/")]
    public async Task<ActionResult<UserCreateDto>> Create([FromBody] UserCreateDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateUserCommand
        {
            Login = request.Login,
            Password = request.Password,
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<UserCreateDto>>(
            u => UserCreateDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }

    [Authorize]
    [HttpPut("update/{userId:guid}")]
    public async Task<ActionResult<UserUpdateDto>> Update([FromRoute] Guid userId, [FromBody] UserUpdateDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateUserCommand
        {
            UserId = userId,
            Login = request.Login,
            Password = request.Password,
            Balance = request.Balance
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<UserUpdateDto>>(
            user => UserUpdateDto.FromDomainModel(user),
            e => e.ToObjectResult());
    }

    [Authorize]
    [HttpDelete("delete/{userId:guid}")]
    public async Task<ActionResult<UserDto>> Delete([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var input = new DeleteUserCommand
        {
            UserId = userId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }
}