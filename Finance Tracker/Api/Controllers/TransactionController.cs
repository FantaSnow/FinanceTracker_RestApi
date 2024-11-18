using Api.Dtos.Transactions;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Transactions.Commands;
using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("transactions")]
[ApiController]
public class TranasctionController(ISender sender, ITransactionQueries transactionQueries) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("getAll/")]
    public async Task<ActionResult<IReadOnlyList<TransactionDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await transactionQueries.GetAll(cancellationToken);

        return entities.Select(TransactionDto.FromDomainModel).ToList();
    }

    
    [AllowAnonymous]
    [HttpGet("getAllByUser/{userId:guid}")]
    public async Task<ActionResult<IReadOnlyList<TransactionDto>>> GetAllByUser([FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var entities = await transactionQueries.GetAllByUser(new UserId(userId), cancellationToken);

        return entities.Select(TransactionDto.FromDomainModel).ToList();
    }
        
    [AllowAnonymous]
    [HttpGet("getAllPlusByUserAndDate/{startDate:datetime}/{endDate:datetime}/user=/{userId:guid}")]
    public async Task<ActionResult<IReadOnlyList<TransactionDto>>> GetAllPlusByUserAndDate(
        [FromRoute] Guid userId, [FromRoute] DateTime startDate, [FromRoute] DateTime endDate,
        CancellationToken cancellationToken)
    {
        var entities = await transactionQueries.GetAllPlusByUserAndDate(new UserId(userId),startDate, endDate,cancellationToken);

        return entities.Select(TransactionDto.FromDomainModel).ToList();
    }

    
    [AllowAnonymous]
    [HttpGet("getAllMinusByUserAndDateAndCategory/{startDate:datetime}/{endDate:datetime}/{categoryId:guid}/user=/{userId:guid}")]
    public async Task<ActionResult<IReadOnlyList<TransactionDto>>> GetAllMinusByUserAndDateAndCategory(
        [FromRoute] Guid userId, [FromRoute] DateTime startDate,[FromRoute] Guid categoryId, [FromRoute] DateTime endDate,
        CancellationToken cancellationToken)
    {
        var entities = await transactionQueries.GetAllMinusByUserAndDate(new UserId(userId),startDate, endDate,new CategoryId(categoryId) ,cancellationToken);

        return entities.Select(TransactionDto.FromDomainModel).ToList();
    }
    
    [AllowAnonymous]
    [HttpGet("getAllPlusByUserAndDateAndCategory/{startDate:datetime}/{endDate:datetime}/{categoryId:guid}/user=/{userId:guid}")]
    public async Task<ActionResult<IReadOnlyList<TransactionDto>>> GetAllPlusByUserAndDateAndCategory(
        [FromRoute] Guid userId, [FromRoute] DateTime startDate, [FromRoute] Guid categoryId, [FromRoute] DateTime endDate,
        CancellationToken cancellationToken)
    {
        var entities = await transactionQueries.GetAllPlusByUserAndDate(new UserId(userId),startDate, endDate,new CategoryId(categoryId),cancellationToken);

        return entities.Select(TransactionDto.FromDomainModel).ToList();
    }

    
    [AllowAnonymous]
    [HttpGet("getAllMinusByUserAndDate/{startDate:datetime}/{endDate:datetime}/user=/{userId:guid}")]
    public async Task<ActionResult<IReadOnlyList<TransactionDto>>> GetAllMinusByUserAndDate(
        [FromRoute] Guid userId, [FromRoute] DateTime startDate, [FromRoute] DateTime endDate,
        CancellationToken cancellationToken)
    {
        var entities = await transactionQueries.GetAllMinusByUserAndDate(new UserId(userId),startDate, endDate,cancellationToken);

        return entities.Select(TransactionDto.FromDomainModel).ToList();
    }

    [AllowAnonymous]
    [HttpGet("getById/{transactionId:guid}")]
    public async Task<ActionResult<TransactionDto>> Get([FromRoute] Guid transactionId,
        CancellationToken cancellationToken)
    {
        var entity = await transactionQueries.GetById(new TransactionId(transactionId), cancellationToken);

        return entity.Match<ActionResult<TransactionDto>>(
            t => TransactionDto.FromDomainModel(t),
            () => NotFound());
    }

    [Authorize]
    [HttpPost("create/{userId:guid}")]
    public async Task<ActionResult<TransactionCreateDto>> Create([FromRoute] Guid userId,
        [FromBody] TransactionCreateDto request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userid");
        var userIdFromToken = new Guid(userIdClaim!.Value);
        var input = new CreateTransactionCommand
        {
            Sum = request.Sum,
            CategoryId = request.CategoryId,
            UserId = userId,
            UserIdFromToken=userIdFromToken
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<TransactionCreateDto>>(
            t => TransactionCreateDto.FromDomainModel(t),
            e => e.ToObjectResult());
    }
    
    [Authorize]
    [HttpDelete("delete/{transactionId:guid}")]
    public async Task<ActionResult<TransactionDto>> Delete([FromRoute] Guid transactionId,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userid");
        var userIdFromToken = new Guid(userIdClaim!.Value);
        var input = new DeleteTransactionCommand
        {
            TransactionId = transactionId,
            UserIdFromToken=userIdFromToken
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<TransactionDto>>(
            u => TransactionDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }

    [Authorize]
    [HttpPut("update/{transactionId:guid}")]
    public async Task<ActionResult<TransactionDto>> Update(
        [FromRoute] Guid transactionId,
        [FromBody] TransactionUpdateDto request,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userid");
        var userIdFromToken = new Guid(userIdClaim!.Value);
        
        var input = new UpdateTransactionCommand
        {
            TransactionId = transactionId,
            Sum = request.Sum,
            CategoryId = request.CategoryId,
            UserIdFromToken = userIdFromToken
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<TransactionDto>>(
            f => TransactionDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }
}