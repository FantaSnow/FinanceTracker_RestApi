using Application.Tickets.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; 


namespace Api.Modules.Errors;

public static class TokenErrorHandler
{
    public static ObjectResult ToObjectResult(this TokenException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                LoginException => StatusCodes.Status401Unauthorized,
                TicketUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("User error handler does not implemented")
            }
        };
    }
}