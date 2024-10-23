using System;
using Luciano.Serafim.Banking.Core.Abstractions.Transactions;

namespace Luciano.Serafim.Banking.Core.Infrastructure;

/// <summary>
/// in memory UoW, does nothing
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    /// <inheritdoc/>
    public async Task CommitTransactionAsync()
    {
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        
    }

    /// <inheritdoc/>
    public T GetSession<T>()
    {
        return default;
    }

    /// <inheritdoc/>
    public async Task RollbackTransactionAsync()
    {
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task StartTransactionAsync()
    {
        await Task.CompletedTask;
    }
}
