using Luciano.Serafim.Banking.Core.Models;
using Luciano.Serafim.Banking.Core.UseCases.Events.Requests;
using Luciano.Serafim.Banking.Core.UseCases.Events.Responses;
using MediatR;
using Luciano.Serafim.Banking.Core.UseCases.Accounts.Requests;
using Luciano.Serafim.Banking.Core.Abstractions.Repositories;

namespace Luciano.Serafim.Banking.Core.UseCases.Events;

public class RunDepositUseCase : IRequestHandler<DepositCommand, Response<DepositResponse>>
{
    private readonly Response<DepositResponse> response;
    private readonly IAccountRepository accountService;
    private readonly IEventRepository eventService;
    private readonly IMediator mediator;

    public RunDepositUseCase(Response<DepositResponse> response, IAccountRepository accountService, IEventRepository eventService, IMediator mediator)
    {
        this.response = response;
        this.accountService = accountService;
        this.eventService = eventService;
        this.mediator = mediator;
    }
    public async Task<Response<DepositResponse>> Handle(DepositCommand request, CancellationToken cancellationToken)
    {
        //get destination Account
        var destination = await accountService.GetAccountById(request.DestinationId);

        //if account does not exists then create account
        if (destination is null)
        {
            destination = (await mediator.Send(new CreateAccountCommand(request.DestinationId))).GetResponseObject();
        }

        //add ammount to destination
        var deposit = (Event)request;

        //add ammount to destination
        deposit = await eventService.CreateEvent(deposit);


        //get destination balance
        var destinationBalance = (await mediator.Send(new GetBalanceQuery(request.DestinationId, true))).GetResponseObject();
        response.SetResponsePayload(new DepositResponse(new AccountBalanceResponse(request.DestinationId, destinationBalance)));

        return response;
    }
}
