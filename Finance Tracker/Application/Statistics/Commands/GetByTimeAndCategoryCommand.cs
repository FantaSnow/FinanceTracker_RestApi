using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Statistics.Exceptions;
using Domain.Categorys;
using Domain.Statistics;
using Domain.Transactions;
using Domain.Users;
using MediatR;


namespace Application.Statistics.Commands;

public record GetByTimeAndCategoryCommand : IRequest<Result<Statistic, StatisticException>>
{
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
    public required Guid CategoryId { get; init; }
    public required Guid UserId { get; init; }
}

public class CreateUserCommandHandler(ICategoryRepository categoryRepository, IUserRepository userRepository, ITransactionRepository transactionRepository)
    : IRequestHandler<GetByTimeAndCategoryCommand, Result<Statistic, StatisticException>>
{
    public async Task<Result<Statistic, StatisticException>> Handle(GetByTimeAndCategoryCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var existingUser = await userRepository.GetById(userId, cancellationToken);
        
        return await existingUser.Match(
            async u =>
            {
                var categoryId = new CategoryId(request.CategoryId); 
                var existingCategory = await categoryRepository.GetById(categoryId, cancellationToken);

                return await existingCategory.Match(
                async c => await CreateEntity(request.StartDate, request.EndDate, categoryId, userId, cancellationToken),
                () =>  Task.FromResult<Result<Statistic, StatisticException>>(new CategoryNotFoundException(categoryId))
                    );
            },
             () =>  Task.FromResult<Result<Statistic, StatisticException>>(new UserNotFoundException(userId))
            );
    }

    private async Task<Result<Statistic, StatisticException>> CreateEntity(
        DateTime startDate,
        DateTime endDate,
        CategoryId categoryId,
        UserId userId,
        CancellationToken cancellationToken)
    {
        try
        {
            var transactions = await transactionRepository.GetAllByUser(userId, cancellationToken);

            var minusTransactions = transactions.Where(t => t.Sum < 0 && t.CategoryId == categoryId && t.CreatedAt >= startDate && t.CreatedAt <= endDate);
            var plusTransactions = transactions.Where(t => t.Sum > 0 && t.CategoryId == categoryId && t.CreatedAt >= startDate && t.CreatedAt <= endDate);

            var plusStatistic = CalculateStatistics(plusTransactions);
            var minusStatistic = CalculateStatistics(minusTransactions);

            var statistic = Statistic.New(minusStatistic.Sum, minusStatistic.TransactionCount, minusStatistic.CategoryCount, plusStatistic.Sum, plusStatistic.TransactionCount, plusStatistic.CategoryCount);

            return statistic;
        }
        catch (Exception exception)
        {
            return new StatisticUnknownException(userId, exception);
        }
    }

        private (decimal Sum, int TransactionCount, int CategoryCount) CalculateStatistics(IEnumerable<Transaction> transactions)
    {
        decimal sum = transactions.Sum(t => t.Sum);
        int transactionCount = transactions.Count();
        int categoryCount = transactions.Select(t => t.CategoryId).Distinct().Count();

        return (sum, transactionCount, categoryCount);
    }

}