namespace Galytix.Test.Dto;

/// <summary>Wraps server-side exception thrown through API to client.</summary>
public class WebApiError
{
    /// <summary>Initializes a new instance of the <see cref="WebApiError"/> class.</summary>
    /// <param name="exception">The exception that has been thrown.</param>
    public WebApiError(Exception exception)
    {
        if (exception is null) throw new ArgumentNullException(nameof(exception));

        Message = exception.Message;
        Name = exception.GetType().Name;
        Detail = exception.ToString();
    }

    /// <summary>Gets or sets exception type name.</summary>
    public string Name { get; set; }

    /// <summary>Gets or sets exception message.</summary>
    public string Message { get; set; }

    /// <summary>Gets or sets exception detail.</summary>
    public string Detail { get; set; }
}