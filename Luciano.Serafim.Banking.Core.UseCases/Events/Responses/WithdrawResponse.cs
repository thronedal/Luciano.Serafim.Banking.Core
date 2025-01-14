namespace Luciano.Serafim.Banking.Core.UseCases.Events.Responses;

/// <summary>
/// TransferResultDto
/// </summary>
public class WithdrawResponse
{
    public WithdrawResponse(AccountBalanceResponse origin)
    {
        Origin = origin;
    }

    /// <summary>
    /// Balance of the origin account
    /// </summary>
    public AccountBalanceResponse Origin { get; internal set; }
}
