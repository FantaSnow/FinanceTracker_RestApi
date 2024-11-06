using Application.Statistics.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class StatisticErrorHandler
{
    public static ObjectResult ToObjectResult(this StatisticException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                UserNotFoundException => StatusCodes.Status404NotFound,
                CategoryNotFoundException => StatusCodes.Status404NotFound,
                StatisticUnknownException => StatusCodes.Status500InternalServerError,
                YouDoNotHaveTheAuthorityToDo => StatusCodes.Status403Forbidden,
                _ => throw new NotImplementedException("Statistic error handler does not implemented")
            }
        };
    }
}