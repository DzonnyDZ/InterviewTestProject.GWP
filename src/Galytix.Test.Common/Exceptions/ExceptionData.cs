namespace Galytix.Test.Common.Exceptions;

/// <summary>Contains static members for working with <see cref="Exception.Data"/></summary>
public static class ExceptionData
{
    /// <summary>Key in <see cref="Exception.Data"/> to store response HTTP sttaus under</summary>
    public const string HttpStatus = nameof(HttpStatus);
}
