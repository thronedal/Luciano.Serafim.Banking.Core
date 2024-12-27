using Luciano.Serafim.Banking.Core.Models;
using Luciano.Serafim.Banking.Events.Models;

namespace Luciano.Serafim.Banking.Events.UseCases.Helpers;

public static class BalanceHelper
{
    public static readonly EventOperation[] DEBIT_OPERATIONS = [EventOperation.OutgoingTransfer, EventOperation.Withdraw];

    /// <summary>
    /// given a initial balance and a list of events calculate de balance
    /// </summary>
    /// <param name="initialBalance"></param>
    /// <param name="events"></param>
    /// <returns></returns>
    public static double CalculateBalance(double initialBalance, IEnumerable<Event> events)
    {
        return initialBalance + events.Sum(e => e.Amount * (DEBIT_OPERATIONS.Contains(e.Operation) ? -1 : 1));
    }
}