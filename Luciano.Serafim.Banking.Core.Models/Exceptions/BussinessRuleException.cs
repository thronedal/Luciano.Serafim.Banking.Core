using Luciano.Serafim.Banking.Core.Models.Exceptions.Abstractions;

namespace Luciano.Serafim.Banking.Core.Models.Exceptions;

/// <summary>
/// Exception raised for bussiness rules erros
/// </summary>
public class BussinessRuleException : BaseException, IConflictException
{

    /// <inheritdoc/>
    public BussinessRuleException(string message) : base("409", message)
    {

    }

    /// <inheritdoc/>
    public BussinessRuleException(string message, Exception innerException) : base("409", message, innerException)
    {

    }

}