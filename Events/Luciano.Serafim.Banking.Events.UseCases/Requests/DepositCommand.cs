using Luciano.Serafim.Banking.Core.Abstractions.Caching;
using Luciano.Serafim.Banking.Core.Models;
using Luciano.Serafim.Banking.Events.UseCases.Responses;
using MediatR;

namespace Luciano.Serafim.Banking.Events.UseCases.Requests;

/// <summary>
/// DepositCommand
/// </summary>
public class DepositCommand : IRequest<Response<DepositResponse>>, ICacheInvalidation
{
    public DepositCommand(int destinationId, double amount)
    {
        DestinationId = destinationId;
        Amount = amount;
    }

    /// <summary>
    /// Id for destination account
    /// </summary>
    public int DestinationId { get; internal set; }

    /// <summary>
    /// amount to be deposited
    /// </summary>
    public double Amount { get; internal set; }

    /// <inheritdoc/>
    public IEnumerable<string> KeysToInvalidate => [$"balance-{DestinationId}"];

    /// <summary>
    /// Converts the base type <see cref="DepositCommand"/> to <see cref="Event"/>.
    /// </summary>
    /// <param name="command"><see cref="DepositCommand"/></param>
    public static explicit operator Event(DepositCommand command)
    {
        return new Event(EventOperation.Deposit, command.Amount, DateTime.UtcNow, command.DestinationId);
    }
}