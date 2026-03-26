using AppCore.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

public class ProblemDetailsExceptionHandler(ProblemDetailsFactory factory, ILogger<ProblemDetailsExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is ContactNotFoundException)
        {
            logger.LogInformation("Handled exception: {Message}", exception.Message);
            var problem = factory.CreateProblemDetails(
                context,
                StatusCodes.Status400BadRequest,
                title: "Contact service error",
                detail: exception.Message
            );
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(problem, cancellationToken);
            return true;
        }
        return false;
    }
}