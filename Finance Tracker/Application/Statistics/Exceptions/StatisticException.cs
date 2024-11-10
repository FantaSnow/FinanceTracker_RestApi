using Domain.Categorys;
using Domain.Users;

namespace Application.Statistics.Exceptions;

public abstract class StatisticException : Exception
{
    public CategoryId? CategoryId { get; }
    public UserId? UserId { get; }

    protected StatisticException(CategoryId? categoryId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        CategoryId = categoryId;
    }

    protected StatisticException(UserId? userId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        UserId = userId;
    }
}
public class YouDoNotHaveTheAuthorityToDo(UserId idFromToken, UserId idFromUser) : StatisticException(idFromToken, $"You do not have the authority to do. Your id:{idFromToken} | Id what u want to change: {idFromUser}");

public class UserNotFoundException(UserId id) : StatisticException(id, $"User under id: {id} not found");

public class CategoryNotFoundException(CategoryId id) : StatisticException(id, $"Category under id: {id} not found");

public class StatisticUnknownException(UserId id, Exception innerException)
    : StatisticException(id, $"Unknown exception for the statistics under Userid: {id}", innerException);