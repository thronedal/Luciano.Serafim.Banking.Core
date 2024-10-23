using Luciano.Serafim.Banking.Core.Abstractions.Caching;
using Luciano.Serafim.Banking.Core.Models;
using MediatR;

namespace Luciano.Serafim.Banking.Core.UseCases.Accounts.Requests;

public class GetBalanceQuery : IRequest<Response<double>>, ICacheable
{

    /// <summary>
    /// Create a Query to get a accouint balance
    /// </summary>
    /// <param name="accountId">if of the account</param>
    /// <param name="ignoreCache">if set to true ignores cache</param>
    public GetBalanceQuery(int accountId, bool ignoreCache = false)
    {
        AccountId = accountId;
        IgnoreCache = ignoreCache;
    }

    /// <summary>
    /// Account Id
    /// </summary>
    public int AccountId { get; internal set; }

    /// <inheritdoc/>
    public string CacheKey => $"balance-{AccountId}";

    public bool IgnoreCache { get; internal set; }
}