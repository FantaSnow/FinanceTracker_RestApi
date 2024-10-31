using Api.Dtos;
using Api.Dtos.Transactions;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Transactions.Commands;
using Domain.Transactions;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("transactions")]
[ApiController]
public class TranasctionController(ISender sender, ITransactionQueries transactionQueries) : ControllerBase
{
    [HttpGet("getAll/")]
    public async Task<ActionResult<IReadOnlyList<TransactionDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await transactionQueries.GetAll(cancellationToken);

        return entities.Select(TransactionDto.FromDomainModel).ToList();
    }
    
    [HttpGet("getAllByUser/{userId:guid}")]
    public async Task<ActionResult<IReadOnlyList<TransactionDto>>> GetAllByUser([FromRoute] Guid userId,CancellationToken cancellationToken)
    {
        var entities = await transactionQueries.GetAllByUser(new UserId(userId),cancellationToken);

        return entities.Select(TransactionDto.FromDomainModel).ToList();
    }
    
    [HttpGet("getById/{transactionId:guid}")]
    public async Task<ActionResult<TransactionDto>> Get([FromRoute] Guid transactionId, CancellationToken cancellationToken)
    {
        var entity = await transactionQueries.GetById(new TransactionId(transactionId), cancellationToken);

        return entity.Match<ActionResult<TransactionDto>>(
            t => TransactionDto.FromDomainModel(t),
            () => NotFound());
    }

    [HttpPost("create/{userId:guid}")]
    public async Task<ActionResult<TransactionCreateDto>> Create([FromRoute] Guid userId,[FromBody] TransactionCreateDto request, CancellationToken cancellationToken)
    {
        var input = new CreateTransactionCommand
        {
            Sum = request.Sum,
            CategoryId = request.CategoryId,
            UserId = userId
        };

        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<TransactionCreateDto>>(
            t => TransactionCreateDto.FromDomainModel(t),
            e => e.ToObjectResult());
    }
    
    [HttpDelete("delete/{transactionId:guid}")]
    public async Task<ActionResult<TransactionDto>> Delete([FromRoute] Guid transactionId, CancellationToken cancellationToken)
    {
        var input = new DeleteTransactionCommand
        {
            TransactionId = transactionId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<TransactionDto>>(
            u => TransactionDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }
    
    [HttpPut("update/{transactionId:guid}")]
    public async Task<ActionResult<TransactionUpdateDto>> Update(
        [FromRoute] Guid transactionId,
        [FromBody] TransactionUpdateDto request,
        CancellationToken cancellationToken)
    {
        
        var input = new UpdateTransactionCommand
        {
            TransactionId = transactionId,
            Sum = request.Sum,
            CategoryId = request.CategoryId,
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<TransactionUpdateDto>>(
            f => TransactionUpdateDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }
}