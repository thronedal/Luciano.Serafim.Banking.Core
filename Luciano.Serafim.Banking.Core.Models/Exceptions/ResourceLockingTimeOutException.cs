using Luciano.Serafim.Banking.Core.Models.Exceptions.Abstractions;

namespace Luciano.Serafim.Banking.Core.Models.Exceptions;

/// <summary>
/// Exception raised when the resource locking fail 
/// </summary>
public class ResourceLockingTimeOutException : BaseException, IConflictException
{
    /// <inheritdoc/>
    public ResourceLockingTimeOutException(string message) : base("409", message)
    {

    }

    /// <inheritdoc/>
    public ResourceLockingTimeOutException(string message, Exception innerException) : base("409", message, innerException)
    {

    }
}