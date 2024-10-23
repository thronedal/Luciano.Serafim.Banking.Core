using Luciano.Serafim.Banking.Core.Abstractions.Repositories;
using Luciano.Serafim.Banking.Core.Models;
using Luciano.Serafim.Banking.Core.Models.Exceptions;
using Luciano.Serafim.Banking.Core.UseCases.Accounts.Requests;
using Luciano.Serafim.Banking.Core.UseCases.Helpers;
using MediatR;

namespace Luciano.Serafim.Banking.Core.UseCases.Accounts;

public class GetBalanceUseCase : IRequestHandler<GetBalanceQuery, Response<double>>
{
    private readonly Response<double> response;
    private readonly IAccountRepository accountService;
    private readonly IEventRepository eventService;

    public GetBalanceUseCase(Response<double> response, IAccountRepository accountService, IEventRepository eventService)
    {
        this.response = response;
        this.accountService = accountService;
        this.eventService = eventService;
    }

    public async Task<Response<double>> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
    {
        //get account
        var account = await accountService.GetAccountById(request.AccountId);

        //account exists?
        if (account is null)
        {
            throw new ObjectNotFoundException("404", request.AccountId.ToString(), nameof(request.AccountId));
        }

        //get balance        
        var consolidatedBalance = await accountService.GetLastConsolidatedBalance(request.AccountId);
        var events = await eventService.GetEventsAfter(account.Id, consolidatedBalance.BalanceDate.ToDateTime(new TimeOnly()));

        //calculate balance
        var balance = BalanceHelper.CalculateBalance(consolidatedBalance.Balance, events);

        response.SetResponsePayload(balance);

        return response;
    }
}
