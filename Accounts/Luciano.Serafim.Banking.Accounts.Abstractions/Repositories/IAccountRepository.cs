using Luciano.Serafim.Banking.Accounts.Models;

namespace Luciano.Serafim.Banking.Accounts.Abstractions.Repositories;

/// <summary>
/// Account service
/// </summary>
public interface IAccountRepository
{
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
}
