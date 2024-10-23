using Luciano.Serafim.Banking.Core.Abstractions.Repositories;
using Luciano.Serafim.Banking.Core.Models;
using Luciano.Serafim.Banking.Core.Models.Exceptions;
using Luciano.Serafim.Banking.Core.UseCases.Accounts.Requests;
using MediatR;

namespace Luciano.Serafim.Banking.Core.UseCases.Accounts;

public class CreateAccountUseCase : IRequestHandler<CreateAccountCommand, Response<Account>>
{
    private readonly Response<Account> response;
    private readonly IAccountRepository accountService;

    public CreateAccountUseCase(Response<Account> response, IAccountRepository accountService)
    {
        this.response = response;
        this.accountService = accountService;
    }
    public async Task<Response<Account>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {

        //check if account exists
        bool exists = await accountService.AccountExists(request.AccountId);

        if (exists)
        {
            throw new BussinessRuleException($"Account Id '{request.AccountId}' already exist and can not be created");
        }

        //creates account
        Account account = (Account)request;
        account = await accountService.CreateAccount(account);

        //consolidate initial balance (0)
        AccountConsolidatedBalance consolidatedBalance = await accountService.ConsolidateBalance(account, DateOnly.FromDateTime(DateTime.UtcNow), 0.0);
        response.SetResponsePayload(account);

        return response;
    }
}
