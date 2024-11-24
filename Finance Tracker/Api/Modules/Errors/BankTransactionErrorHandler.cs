using Application.BankTransactions.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Api.Modules.Errors;

public static class BankTransactionErrorHandler
{
    public static ObjectResult ToObjectResult(this BankTransactionException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                BankNotFoundException => StatusCodes.Status404NotFound,
                UserNotFoundException => StatusCodes.Status404NotFound,
                BankTransactionNotFoundException => StatusCodes.Status404NotFound,
                BankTransactionAlreadyExistsException => StatusCodes.Status409Conflict,
                BankTransactionUnknownException => StatusCodes.Status500InternalServerError,
                YouDoNotHaveTheAuthorityToDo => StatusCodes.Status403Forbidden,
                _ => throw new NotImplementedException("BankTransaction error handler does not implemented")
            }
        };
    }
}