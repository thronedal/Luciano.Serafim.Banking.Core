namespace Luciano.Serafim.Banking.Core.Abstractions.Transactions;

public interface IAcidEnabled
{
    public bool IsDryRun => false;
}
