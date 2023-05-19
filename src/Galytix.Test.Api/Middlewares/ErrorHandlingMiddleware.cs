using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using Galytix.Test.Common.Exceptions;
using Galytix.Test.Dto;

namespace Galytix.Test.Api.Middlewares;

/// <summary>Middleware that handles known errors</summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate next;

    /// <summary>Initializes a new instance of the <see cref="ErrorHandlingMiddleware"/> class.</summary>
    /// <param name="next">Next handler in row.</param>
    public ErrorHandlingMiddleware(RequestDelegate next) => this.next = next ?? throw new ArgumentNullException(nameof(next));

    /// <summary>Invokes next handler.</summary>
    /// <param name="context">Current HTTP context.</param>
    /// <returns>Task to await to wait for the operation to complete.</returns>
    public async Task Invoke(HttpContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));
        try
        {
            await this.next(context);
        }
        catch (Exception ex)
        {
            await this.HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>Handles an exception.</summary>
    /// <param name="context">Current HTTP context.</param>
    /// <param name="exception">Exception to be handled.</param>
    /// <returns>Task to await to wait for the operation to complete.</returns>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = this.GetCode(exception);
        await this.WriteExceptionAsync(context, exception, code).ConfigureAwait(false);
    }

    /// <summary>Gets HTTP status code for exception.</summary>
    /// <param name="exception">Exception to get code for.</param>
    /// <returns>Exception code as <see cref="HttpStatusCode"/>.</returns>
    protected HttpStatusCode GetCode(Exception exception)
    {
        if (exception is null) throw new ArgumentNullException(nameof(exception));

        if (exception.Data.Contains(ExceptionData.HttpStatus))
        {
            var status = exception.Data[ExceptionData.HttpStatus];
            switch (status)
            {
                case int i: return (HttpStatusCode)i;
                case HttpStatusCode sc: return sc;
                case string s:
                    if (Enum.TryParse<HttpStatusCode>(s, true, out var ret))
                    {
                        return ret;
                    }

                    break;

                    // Otherwise nothing
            }
        }

        return exception switch
        {
            ValidationException _ => HttpStatusCode.BadRequest,
            System.Data.RowNotInTableException _ => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError,
        };
    }

    /// <summary>Writes exception details to response.</summary>
    /// <param name="context">Current HTTP context.</param>
    /// <param name="exception">Exception to write.</param>
    /// <param name="code">Response HTTP status code. Not written if null.</param>
    /// <returns>Task to await to wait for the operation to complete.</returns>
    public async Task WriteExceptionAsync(HttpContext context, Exception exception, HttpStatusCode? code)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));

        var response = context.Response;
        response.ContentType = "application/json";
        if (code.HasValue)
        {
            response.StatusCode = (int)code;
        }

        string errorMessage = JsonSerializer.Serialize(new WebApiError(exception));

        await response.WriteAsync(errorMessage).ConfigureAwait(false);
    }
}
