using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Statistics.Exceptions;
using Domain.Categorys;
using Domain.Statistics;
using Domain.Transactions;
using Domain.Users;
using MediatR;


namespace Application.Statistics.Commands;

public record GetByTimeAndCategoryForAllCommand : IRequest<Result<Statistic, StatisticException>>
{
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
    public required Guid UserId { get; init; }
    public required Guid UserIdFromToken { get; init; }
}

public class GetByTimeAndCategoryForAllCommandHandler(
    ICategoryRepository categoryRepository,
    IUserRepository userRepository,
    ITransactionRepository transactionRepository)
    : IRequestHandler<GetByTimeAndCategoryForAllCommand, Result<Statistic, StatisticException>>
{
    public async Task<Result<Statistic, StatisticException>> Handle(GetByTimeAndCategoryForAllCommand request,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var existingUserForStatistic = await userRepository.GetById(userId, cancellationToken);

        return await existingUserForStatistic.Match<Task<Result<Statistic, StatisticException>>>(
            async ufs =>
            {
                var userIdFromToken = new UserId(request.UserIdFromToken);
                var existingUserFromToken = await userRepository.GetById(userIdFromToken, cancellationToken);

                return await existingUserFromToken.Match<Task<Result<Statistic, StatisticException>>>(
                    async uft =>await CreateEntity(request.StartDate, request.EndDate, ufs, uft, cancellationToken),
                    () => Task.FromResult<Result<Statistic, StatisticException>>(new UserNotFoundException(userId)));
            },
            () => Task.FromResult<Result<Statistic, StatisticException>>(new UserNotFoundException(userId))
        );
    }

    private async Task<Result<Statistic, StatisticException>> CreateEntity(
        DateTime startDate,
        DateTime endDate,
        User userForStatistic,
        User userFromToken,
        CancellationToken cancellationToken)
    {
        try
        {
            if (userFromToken.Id == userForStatistic.Id || userFromToken.IsAdmin)
            {
                var transactions = await transactionRepository.GetAllByUser(userForStatistic.Id, cancellationToken);

                var minusTransactions = transactions.Where(t =>
                    t.Sum < 0  && t.CreatedAt >= startDate && t.CreatedAt <= endDate);
                var plusTransactions = transactions.Where(t =>
                    t.Sum > 0 && t.CreatedAt >= startDate && t.CreatedAt <= endDate);

                var plusStatistic = CalculateStatistics(plusTransactions);
                var minusStatistic = CalculateStatistics(minusTransactions);

                var statistic = Statistic.New(minusStatistic.Sum, minusStatistic.TransactionCount,
                    minusStatistic.CategoryCount, plusStatistic.Sum, plusStatistic.TransactionCount,
                    plusStatistic.CategoryCount);

                return statistic;
            }
            return await Task.FromResult<Result<Statistic, StatisticException>>(new YouDoNotHaveTheAuthorityToDo(userFromToken.Id, userForStatistic.Id));
            
        }
        catch (Exception exception)
        {
            return new StatisticUnknownException(userForStatistic.Id, exception);
        }
    }

    private (decimal Sum, int TransactionCount, int CategoryCount) CalculateStatistics(
        IEnumerable<Transaction> transactions)
    {
        decimal sum = transactions.Sum(t => t.Sum);
        int transactionCount = transactions.Count();
        int categoryCount = transactions.Select(t => t.CategoryId).Distinct().Count();

        return (sum, transactionCount, categoryCount);
    }
}