using System;

namespace Luciano.Serafim.Banking.Core.Models;

/// <summary>
/// Define the account Entity
/// </summary>
public class Account
{

    public Account(int id, string accountNumber)
    {
        Id = id;
        AccountNumber = accountNumber;
    }
    /// <summary>
    /// Surrogate Key generated to identify the account, 0 (zero) indicates a new account
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Account Number
    /// </summary>
    public string AccountNumber { get; private set; }
}
