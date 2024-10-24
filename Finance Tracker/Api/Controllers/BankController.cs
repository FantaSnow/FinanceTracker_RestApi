
using Api.Dtos;
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
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BankDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await bankQueries.GetAll(cancellationToken);

        return entities.Select(BankDto.FromDomainModel).ToList();
    }
    
    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<IReadOnlyList<BankDto>>> GetAllByUser([FromRoute] Guid userId,CancellationToken cancellationToken)
    {
        var entities = await bankQueries.GetAllByUser(new UserId(userId),cancellationToken);

        return entities.Select(BankDto.FromDomainModel).ToList();
    }
    
    [HttpGet("bank/{bankId:guid}")]
    public async Task<ActionResult<BankDto>> Get([FromRoute] Guid bankId, CancellationToken cancellationToken)
    {
        var entity = await bankQueries.GetById(new BankId(bankId), cancellationToken);

        return entity.Match<ActionResult<BankDto>>(
            u => BankDto.FromDomainModel(u),
            () => NotFound());
    }

    [HttpPost]
    public async Task<ActionResult<BankDto>> Create([FromBody] BankDto request, CancellationToken cancellationToken)
    {
        var input = new CreateBankCommand
        {
            Name = request.Name,
            BalanceGoal = request.Balance,
            UserId = request.UserId
        };

        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<BankDto>>(
            f => BankDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }
    
    [HttpPut("bank/{bankId:guid}/{balanceToAdd:decimal}")]
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

    
    [HttpDelete("{bankId:guid}")]
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
    
    [HttpPut]
    public async Task<ActionResult<BankDto>> Update(
        [FromBody] BankDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateBankCommand
        {
            BankId = request.BankId!.Value,
            Name = request.Name,
            BalanceGoal = request.BalanceGoal
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<BankDto>>(
            f => BankDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }
}