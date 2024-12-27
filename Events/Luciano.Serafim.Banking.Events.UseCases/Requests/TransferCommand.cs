using Luciano.Serafim.Banking.Core.Abstractions.Transactions;
using Luciano.Serafim.Banking.Core.Models;
using Luciano.Serafim.Banking.Events.UseCases.Responses;
using MediatR;

namespace Luciano.Serafim.Banking.Events.UseCases.Requests;

public class TransferCommand : IRequest<Response<TransferResponse>>, IAcidEnabled
{
    public TransferCommand(int originId, int destinationId, double amount)
    {
        OriginId = originId;
        DestinationId = destinationId;
        Amount = amount;
    }

    /// <summary>
    /// Origin account Id
    /// </summary>
    public int OriginId { get; internal set; }

    /// <summary>
    /// destination account Id
    /// </summary>
    public int DestinationId { get; internal set; }

    /// <summary>
    /// Operation Amount
    /// </summary>
    public double Amount { get; internal set; }

    /// <summary>
    /// Converts the base type <see cref="TransferCommand"/> to <see cref="Event[]"/>.
    /// </summary>
    /// <param name="command"><see cref="TransferCommand"/></param>
    public static explicit operator Event[](TransferCommand command)
    {
        var events = new Event[]
        {
            new Event(EventOperation.OutgoingTransfer, command.Amount, DateTime.UtcNow, command.OriginId),
            new Event(EventOperation.IncommingTransfer, command.Amount, DateTime.UtcNow, command.DestinationId)
        };

        return events;
    }
}