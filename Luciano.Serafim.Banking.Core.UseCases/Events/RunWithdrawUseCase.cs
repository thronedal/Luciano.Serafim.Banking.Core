using System;
using Luciano.Serafim.Banking.Core.Models.Exceptions;
using Luciano.Serafim.Banking.Core.Models;
using Luciano.Serafim.Banking.Core.UseCases.Events.Requests;
using Luciano.Serafim.Banking.Core.UseCases.Events.Responses;
using MediatR;
using Luciano.Serafim.Banking.Core.UseCases.Accounts.Requests;
using Luciano.Serafim.Banking.Core.Abstractions.Repositories;

namespace Luciano.Serafim.Banking.Core.UseCases.Events;

public class RunWithdrawUseCase : IRequestHandler<WithdrawCommand, Response<WithdrawResponse>>
{
    private readonly Response<WithdrawResponse> response;
    private readonly IAccountRepository accountService;
    private readonly IEventRepository eventService;
    private readonly IMediator mediator;

    public RunWithdrawUseCase(Response<WithdrawResponse> response, IAccountRepository accountService, IEventRepository eventService, IMediator mediator)
    {
        this.response = response;
        this.accountService = accountService;
        this.eventService = eventService;
        this.mediator = mediator;
    }

    public async Task<Response<WithdrawResponse>> Handle(WithdrawCommand request, CancellationToken cancellationToken)
    {

        //get origin account 
        var origin = await accountService.GetAccountById(request.OriginId);

        //if account does not exists throws error
        if (origin is null)
        {
            throw new ObjectNotFoundException("404", request.OriginId.ToString(), nameof(request.OriginId));
        }

        //calculate balance
        var balance = (await mediator.Send(new GetBalanceQuery(origin.Id))).GetResponseObject();

        //validate origin balance
        if (balance < request.Amount)
        {
            throw new BussinessRuleException($"Balance '{balance:C2}' should be higher than the operation amount $'{request.Amount:C2}'");
        }

        var withdraw = (Event)request;

        //remove ammount from origin
        withdraw = await eventService.CreateEvent(withdraw);

        //get origin balance
        var originBalance = (await mediator.Send(new GetBalanceQuery(request.OriginId, true))).GetResponseObject();
        response.SetResponsePayload(new WithdrawResponse(new AccountBalanceResponse(request.OriginId, originBalance)));

        return await Task.FromResult(response);
    }
}
