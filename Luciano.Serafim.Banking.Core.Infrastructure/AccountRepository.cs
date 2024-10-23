using Luciano.Serafim.Banking.Core.Abstractions.Repositories;
using Luciano.Serafim.Banking.Core.Models;

namespace Luciano.Serafim.Banking.Core.Infrastructure;

public class AccountRepository : IAccountRepository
{
    private readonly List<Account> accounts = new();
    private readonly List<AccountConsolidatedBalance> consolidatedBalances = new();

    public AccountRepository()
    {
        var _ = InitializeState().Result;
    }

    /// <inheritdoc/>
    public async Task<bool> InitializeState()
    {
        accounts.Clear();
        consolidatedBalances.Clear();

        //creates accounts from 1 to 99 and 300
        accounts.AddRange(Enumerable.Range(1, 99).Select(i => new Account(i, i.ToString())).ToList());
        accounts.Add(new Core.Models.Account(300, "300"));

        //creates consolidated balance for today, with the balance equal to the account id    
        consolidatedBalances.AddRange(accounts.Select(a => new AccountConsolidatedBalance(a, DateOnly.FromDateTime(DateTime.UtcNow), a.Id)));

        return await Task.FromResult(true);
    }

    /// <inheritdoc/>
    public async Task<bool> AccountExists(int accountId)
    {
        return await Task.FromResult(accounts.Exists(a => a.Id == accountId));
    }

    /// <inheritdoc/>
    public async Task<AccountConsolidatedBalance> ConsolidateBalance(Account account, DateOnly date, double balance)
    {
        var consolidated = new AccountConsolidatedBalance(account, DateOnly.FromDateTime(DateTime.UtcNow), balance);
        consolidatedBalances.Add(consolidated);

        return await Task.FromResult(consolidated);
    }

    /// <inheritdoc/>
    public async Task<Core.Models.Account> CreateAccount(Account account)
    {
        accounts.Add(account);

        return await Task.FromResult(account);
    }

    /// <inheritdoc/>
    public async Task<Core.Models.Account?> GetAccountById(int accountId)
    {
        return await Task.FromResult(accounts.Where(a => a.Id == accountId).FirstOrDefault());
    }

    /// <inheritdoc/>
    public async Task<AccountConsolidatedBalance> GetLastConsolidatedBalance(int accountId)
    {
        return await Task.FromResult(consolidatedBalances.Where(c => c.Account.Id == accountId 
                                         && c.BalanceDate == consolidatedBalances.Where(c => c.Account.Id == accountId).Max(m => m.BalanceDate))
                                   .FirstOrDefault(new AccountConsolidatedBalance(new Core.Models.Account(accountId, accountId.ToString()), DateOnly.MinValue, 0)));
                            
    }
}
