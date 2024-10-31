using Api.Dtos.Banks;
using Api.Modules.Errors;
using Application.Banks.Commands;
using Application.Common.Interfaces.Queries;
using Domain.Banks;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("banks")]
[ApiController]
public class BankController(ISender sender, IBankQueries bankQueries) : ControllerBase
{
    [HttpGet("getAll")]
    public async Task<ActionResult<IReadOnlyList<BankDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await bankQueries.GetAll(cancellationToken);

        return entities.Select(BankDto.FromDomainModel).ToList();
    }

    [HttpGet("getAllByUser/{userId:guid}")]
    public async Task<ActionResult<IReadOnlyList<BankDto>>> GetAllByUser([FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var entities = await bankQueries.GetAllByUser(new UserId(userId), cancellationToken);

        return entities.Select(BankDto.FromDomainModel).ToList();
    }

    [HttpGet("getById/{bankId:guid}")]
    public async Task<ActionResult<BankDto>> Get([FromRoute] Guid bankId, CancellationToken cancellationToken)
    {
        var entity = await bankQueries.GetById(new BankId(bankId), cancellationToken);

        return entity.Match<ActionResult<BankDto>>(
            u => BankDto.FromDomainModel(u),
            () => NotFound());
    }

    [HttpPost("create/{userId:guid}")]
    public async Task<ActionResult<BankCreateDto>> Create([FromRoute] Guid userId, [FromBody] BankCreateDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateBankCommand
        {
            Name = request.Name,
            BalanceGoal = request.BalanceGoal,
            UserId = userId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<BankCreateDto>>(
            f => BankCreateDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }

    [HttpPut("addToBalance/{bankId:guid}/{balanceToAdd:decimal}")]
    public async Task<ActionResult<BankDto>> AddToBalance(
        [FromRoute] Guid bankId,
        [FromRoute] decimal balanceToAdd,
        CancellationToken cancellationToken)
    {
        var input = new AddBankBalanceCommand
        {
            BankId = bankId,
            BalanceToAdd = balanceToAdd
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<BankDto>>(
            f => BankDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }


    [HttpDelete("delete/{bankId:guid}")]
    public async Task<ActionResult<BankDto>> Delete([FromRoute] Guid bankId, CancellationToken cancellationToken)
    {
        var input = new DeleteBankCommand
        {
            BankId = bankId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<BankDto>>(
            u => BankDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }

    [HttpPut("update/{bankId:guid}")]
    public async Task<ActionResult<BankUpdateDto>> Update(
        [FromRoute] Guid bankId,
        [FromBody] BankUpdateDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateBankCommand
        {
            BankId = bankId,
            Name = request.Name,
            BalanceGoal = request.BalanceGoal
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<BankUpdateDto>>(
            f => BankUpdateDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }
}