using Api.Dtos.Banks;
using Api.Modules.Errors;
using Application.Banks.Commands;
using Application.Common.Interfaces.Queries;
using Domain.Banks;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("banks")]
[ApiController]
public class BankController(ISender sender, IBankQueries bankQueries) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("getAll")]
    public async Task<ActionResult<IReadOnlyList<BankDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await bankQueries.GetAll(cancellationToken);

        return entities.Select(BankDto.FromDomainModel).ToList();
    }

    [AllowAnonymous]
    [HttpGet("getAllByUser/{userId:guid}")]
    public async Task<ActionResult<IReadOnlyList<BankDto>>> GetAllByUser([FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var entities = await bankQueries.GetAllByUser(new UserId(userId), cancellationToken);

        return entities.Select(BankDto.FromDomainModel).ToList();
    }

    [AllowAnonymous]
    [HttpGet("getById/{bankId:guid}")]
    public async Task<ActionResult<BankDto>> Get([FromRoute] Guid bankId, CancellationToken cancellationToken)
    {
        var entity = await bankQueries.GetById(new BankId(bankId), cancellationToken);

        return entity.Match<ActionResult<BankDto>>(
            u => BankDto.FromDomainModel(u),
            () => NotFound());
    }

    [Authorize]
    [HttpPost("create/{userId:guid}")]
    public async Task<ActionResult<BankDto>> Create([FromRoute] Guid userId, [FromBody] BankCreateDto request,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userid");
        var userIdFromToken = new Guid(userIdClaim!.Value);
        
        var input = new CreateBankCommand
        {
            Name = request.Name,
            BalanceGoal = request.BalanceGoal,
            UserId = userId,
            UserIdFromToken = userIdFromToken
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<BankDto>>(
            f => BankDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }


    [Authorize]
    [HttpDelete("delete/{bankId:guid}")]
    public async Task<ActionResult<BankDto>> Delete([FromRoute] Guid bankId, CancellationToken cancellationToken)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userid");
        var userId = new Guid(userIdClaim!.Value);
        
        var input = new DeleteBankCommand
        {
            BankId = bankId,
            UserIdFromToken = userId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<BankDto>>(
            u => BankDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }

    [Authorize]
    [HttpPut("update/{bankId:guid}")]
    public async Task<ActionResult<BankDto>> Update(
        [FromRoute] Guid bankId,
        [FromBody] BankUpdateDto request,
        CancellationToken cancellationToken)
    {
        
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userid");
        var userIdFromToken = new Guid(userIdClaim!.Value);

        var input = new UpdateBankCommand
        {
            BankId = bankId,
            Name = request.Name,
            BalanceGoal = request.BalanceGoal,
            UserIdFromToken = userIdFromToken
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<BankDto>>(
            f => BankDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }
}