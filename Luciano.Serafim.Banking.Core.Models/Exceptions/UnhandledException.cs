using Luciano.Serafim.Banking.Core.Models.Exceptions.Abstractions;

namespace Luciano.Serafim.Banking.Core.Models.Exceptions;

/// <summary>
/// Exception raised for errors not expected
/// </summary>
public class UnhandledException : BaseException, IInternalServerErrorException
{
    private const string message = "Something is not working as intended, contact IT";

    /// <inheritdoc/>
    public UnhandledException() : base("500", message)
    {

    }

    /// <inheritdoc/>
    public UnhandledException(Exception innerException) : base("500", message, innerException)
    {

    }

}