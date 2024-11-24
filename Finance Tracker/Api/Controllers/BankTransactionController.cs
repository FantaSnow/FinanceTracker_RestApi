using Api.Dtos.BankTransactions;
using Api.Dtos.Transactions;
using Api.Modules.Errors;
using Application.BankTransactions.Commands;
using Application.Common.Interfaces.Queries;
using Domain.Banks;
using Domain.BankTransactions;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("bankTransactions")]
[ApiController]
public class BankTranasctionController(ISender sender, IBankTransactionQueries bankTransactionQueries) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("getAll/")]
    public async Task<ActionResult<IReadOnlyList<BankTransactionDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await bankTransactionQueries.GetAll(cancellationToken);

        return entities.Select(BankTransactionDto.FromDomainModel).ToList();
    }

    [AllowAnonymous]
    [HttpGet("getAllByBank/{bankId:guid}")]
    public async Task<ActionResult<IReadOnlyList<BankTransactionDto>>> GetAllByBankId([FromRoute] Guid bankId,
        CancellationToken cancellationToken)
    {
        var entities = await bankTransactionQueries.GetAllByBank(new BankId(bankId), cancellationToken);

        return entities.Select(BankTransactionDto.FromDomainModel).ToList();
    }
    
    [AllowAnonymous]
    [HttpGet("getAllByUser/{userId:guid}")]
    public async Task<ActionResult<IReadOnlyList<BankTransactionDto>>> GetAllByUser([FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var entities = await bankTransactionQueries.GetAllByUser(new UserId(userId), cancellationToken);

        return entities.Select(BankTransactionDto.FromDomainModel).ToList();
    }
        
    [AllowAnonymous]
    [HttpGet("getById/{bankTransactionId:guid}")]
    public async Task<ActionResult<BankTransactionDto>> Get([FromRoute] Guid bankTransactionId,
        CancellationToken cancellationToken)
    {
        var entity = await bankTransactionQueries.GetById(new BankTransactionId(bankTransactionId), cancellationToken);

        return entity.Match<ActionResult<BankTransactionDto>>(
            t => BankTransactionDto.FromDomainModel(t),
            () => NotFound());
    }

    [Authorize]
    [HttpPost("create/{userId:guid}/{bankId:guid}")]
    public async Task<ActionResult<BankTransactionCreateDto>> Create([FromRoute] Guid userId, [FromRoute] Guid bankId,
        [FromBody] BankTransactionCreateDto request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userid");
        var userIdFromToken = new Guid(userIdClaim!.Value);
        
        var input = new CreateBankTransactionCommand
        {
            Sum = request.Amount,
            UserId = userId,
            UserIdFromToken = userIdFromToken,
            BankId = bankId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<BankTransactionCreateDto>>(
            t => BankTransactionCreateDto.FromDomainModel(t),
            e => e.ToObjectResult());
    }
    
    [Authorize]
    [HttpDelete("delete/{bankTransactionId:guid}")]
    public async Task<ActionResult<BankTransactionDto>> Delete([FromRoute] Guid bankTransactionId,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userid");
        var userIdFromToken = new Guid(userIdClaim!.Value);
        var input = new DeleteBankTransactionCommand
        {
            UserIdFromToken = userIdFromToken,
            BankTransactionId = bankTransactionId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<BankTransactionDto>>(
            u => BankTransactionDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }

    [Authorize]
    [HttpPut("update/{bankTransactionId:guid}")]
    public async Task<ActionResult<BankTransactionDto>> Update(
        [FromRoute] Guid bankTransactionId,
        [FromBody] BankTransactionUpdateDto request,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userid");
        var userIdFromToken = new Guid(userIdClaim!.Value);
        
        var input = new UpdateBankTransactionCommand
        {
            Sum = request.Amount,
            UserIdFromToken = userIdFromToken,
            BankTransactionId = bankTransactionId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<BankTransactionDto>>(
            f => BankTransactionDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }
}