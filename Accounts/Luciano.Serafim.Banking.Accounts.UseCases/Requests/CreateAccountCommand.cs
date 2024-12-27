using Luciano.Serafim.Banking.Accounts.Models;
using Luciano.Serafim.Banking.Core.Abstractions.Transactions;
using Luciano.Serafim.Banking.Core.Models;
using MediatR;

namespace Luciano.Serafim.Banking.Accounts.UseCases.Requests;

public class CreateAccountCommand : IRequest<Response<Account>>, IAcidEnabled
{
    public CreateAccountCommand(int accountId)
    {
        AccountId = accountId;
    }

    /// <summary>
    /// Account id to be created
    /// </summary>
    public int AccountId { get; internal set; }

    /// <summary>
    /// Converts the base type <see cref="CreateAccountCommand"/> to <see cref="Event"/>.
    /// </summary>
    /// <param name="command"><see cref="CreateAccountCommand"/></param>
    public static explicit operator Account(CreateAccountCommand command)
    {
        return new Account(command.AccountId, command.AccountId.ToString());
    }
}
