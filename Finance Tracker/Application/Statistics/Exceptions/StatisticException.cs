using Domain.Categorys;
using Domain.Users;

namespace Application.Statistics.Exceptions;

public abstract class StatisticException : Exception
{
    public CategoryId? CategoryId { get; }
    public UserId? UserId { get; }

    protected StatisticException(CategoryId? bankId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        CategoryId = bankId;
    }

    protected StatisticException(UserId? userId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        UserId = userId;
    }
}

public class UserNotFoundException(UserId id) : StatisticException(id, $"User under id: {id} not found");

public class CategoryNotFoundException(CategoryId id) : StatisticException(id, $"Category under id: {id} not found");

public class StatisticUnknownException(UserId id, Exception innerException)
    : StatisticException(id, $"Unknown exception for the statistics under Userid: {id}", innerException);