using Application.Banks.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Domain.Banks;
using MediatR;

namespace Application.Banks.Commands;

public record UpdateBankCommand : IRequest<Result<Bank, BankException>>
{
    public required Guid BankId { get; init; }
    public required string Name { get; init; }
    public required decimal BalanceGoal { get; init; }
}

public class UpdateBankCommandHandler(IBankRepository bankRepository)
    : IRequestHandler<UpdateBankCommand, Result<Bank, BankException>>
{
    public async Task<Result<Bank, BankException>> Handle(UpdateBankCommand request,
        CancellationToken cancellationToken)
    {
        var bankId = new BankId(request.BankId);

        var existingBank = await bankRepository.GetById(bankId, cancellationToken);

        return await existingBank.Match(
            async b => await UpdateEntity(b, request.Name, request.BalanceGoal, cancellationToken),
            () => Task.FromResult<Result<Bank, BankException>>(new BankNotFoundException(bankId)));
    }

    private async Task<Result<Bank, BankException>> UpdateEntity(
        Bank entity,
        string name,
        decimal balanceGoal,
        CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateDatails(name, balanceGoal);

            return await bankRepository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new BankUnknownException(entity.Id, exception);
        }
    }
}