namespace Luciano.Serafim.Banking.Core.Models.Enums;

/// <summary>
/// Operation Types
/// </summary>
public enum EventType
{
    /// <summary>
    /// Cash-in a amount into a account
    /// </summary>
    Deposit,

    /// <summary>
    /// Cash-out a amount from a account
    /// </summary>
    Withdraw,

    /// <summary>
    /// transfer a amonunt between two accounts
    /// </summary>
    Transfer
}
