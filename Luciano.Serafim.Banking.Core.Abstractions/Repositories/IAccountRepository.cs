using Luciano.Serafim.Banking.Core.Models;

namespace Luciano.Serafim.Banking.Core.Abstractions.Repositories;

/// <summary>
/// Account service
/// </summary>
public interface IAccountRepository
{

    /// <summary>
    /// initialize the app state, for testing purposes when using in memory storage
    /// </summary>
    /// <returns></returns>
    Task<bool> InitializeState();

    /// <summary>
    /// return the last consolidated balance for the account
    /// </summary>
    /// <param name="accountId">account Id</param>
    /// <returns></returns>
    Task<AccountConsolidatedBalance> GetLastConsolidatedBalance(int accountId);


    /// <summary>
    /// return account info by its Id
    /// </summary>
    /// <param name="accountId">account Id</param>
    /// <returns></returns>
    Task<Account?> GetAccountById(int accountId);

    /// <summary>
    /// presists a new account
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    Task<Account> CreateAccount(Account account);

    /// <summary>
    /// Check if an account exists
    /// </summary>
    /// <param name="accountId"></param>
    /// <returns></returns>
    Task<bool> AccountExists(int accountId);

    /// <summary>
    /// consolidate balance for the day
    /// </summary>
    /// <param name="id"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    Task<AccountConsolidatedBalance> ConsolidateBalance(Account account, DateOnly date, double balance);
}
