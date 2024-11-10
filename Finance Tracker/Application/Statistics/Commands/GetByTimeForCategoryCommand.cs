using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Statistics.Exceptions;
using Domain.Statistics;
using Domain.Users;
using MediatR;


namespace Application.Statistics.Commands;

public record GetByTimeForCategoryCommand : IRequest<Result<List<StatisticCategory>, StatisticException>>
{
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
    public required Guid UserId { get; init; }
    public required Guid UserIdFromToken { get; init; }
}

public class GetByTimeForCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUserRepository userRepository,
    ITransactionRepository transactionRepository)
    : IRequestHandler<GetByTimeForCategoryCommand, Result<List<StatisticCategory>, StatisticException>>
{
    public async Task<Result<List<StatisticCategory>, StatisticException>> Handle(GetByTimeForCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var existingUserForStatistic = await userRepository.GetById(userId, cancellationToken);

        return await existingUserForStatistic.Match<Task<Result<List<StatisticCategory>, StatisticException>>>(
            async ufs =>
            {
                var userIdFromToken = new UserId(request.UserIdFromToken);
                var existingUserFromToken = await userRepository.GetById(userIdFromToken, cancellationToken);

                return await existingUserFromToken.Match<Task<Result<List<StatisticCategory>, StatisticException>>>(
                    async uft =>
                    {
                        return  await CreateEntity(request.StartDate, request.EndDate, ufs,uft, cancellationToken);
                    },
                    () => Task.FromResult<Result<List<StatisticCategory>, StatisticException>>(new UserNotFoundException(userIdFromToken)));
            }, 
            () => Task.FromResult<Result<List<StatisticCategory>, StatisticException>>(new UserNotFoundException(userId)));
    }

    private async Task<Result<List<StatisticCategory>, StatisticException>> CreateEntity(
        DateTime startDate,
        DateTime endDate,
        User userForStatistics,
        User userFromToken,
        CancellationToken cancellationToken)
    {
        try
        {
            if (userFromToken.Id == userForStatistics.Id || userFromToken.IsAdmin)
            {
                var categories = await categoryRepository.GetAll(cancellationToken);

                var transactions = await transactionRepository.GetAllByUser(userForStatistics.Id, cancellationToken);

                var statistics = categories.Select(category =>
                {
                    var categoryTransactions = transactions
                        .Where(t => t.CategoryId == category.Id && t.CreatedAt >= startDate && t.CreatedAt <= endDate);

                    var plusSum = categoryTransactions.Where(t => t.Sum > 0).Sum(t => t.Sum);
                    var minusSum = categoryTransactions.Where(t => t.Sum < 0).Sum(t => t.Sum);
                    var transactionCount = categoryTransactions.Count();

                    return StatisticCategory.New(
                        categoryName: category.Name,
                        minusSum: minusSum,
                        plusSum: plusSum,
                        coutTransaction: transactionCount
                    );
                }).ToList();

                return statistics;
            }

            return await Task.FromResult<Result<List<StatisticCategory>, StatisticException>>(
                new YouDoNotHaveTheAuthorityToDo(userFromToken.Id, userForStatistics.Id)
            );            
        }
        catch (Exception exception)
        {
            return new StatisticUnknownException(userForStatistics.Id, exception);
        }
    }
}