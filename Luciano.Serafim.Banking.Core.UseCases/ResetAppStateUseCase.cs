using Luciano.Serafim.Banking.Core.Abstractions.Repositories;
using Luciano.Serafim.Banking.Core.Models;
using MediatR;

namespace Luciano.Serafim.Banking.Core.UseCases;

public class ResetAppStateUseCase : IRequestHandler<ResetAppStateCommand, Response<bool>>
{
    private readonly Response<bool> response;
    private readonly IAccountRepository accountService;
    private readonly IEventRepository eventService;

    public ResetAppStateUseCase(Response<bool> response, IAccountRepository accountService, IEventRepository eventService)
    {
        this.response = response;
        this.accountService = accountService;
        this.eventService = eventService;
    }

    public async Task<Response<bool>> Handle(ResetAppStateCommand request, CancellationToken cancellationToken)
    {
        await accountService.InitializeState();
        await eventService.InitializeState();

        response.SetResponsePayload(true);

        return response;
    }
}

public class ResetAppStateCommand : IRequest<Response<bool>>
{
}