using Luciano.Serafim.Banking.Core.Models.Exceptions;
using Luciano.Serafim.Banking.Core.Tests.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Luciano.Serafim.Banking.Core.UseCases.Events.Requests;

namespace Luciano.Serafim.Banking.Core.Tests.Core.UseCases.Events;

public class RunTransferUseCaseTests
{
    private readonly IMediator mediator;

    public RunTransferUseCaseTests()
    {
        var services = new ServiceCollection();
        var serviceProvider = services
            .AddTest()
            .BuildServiceProvider();

        mediator = serviceProvider.GetRequiredService<IMediator>();
    }

    //# Transfer from existing account
    [Theory]
    [InlineData(50, 90, 30)]
    public async Task RunTransfer_Success(int originId, int destinationId, double amount)
    {
        TransferCommand command = new( originId, destinationId, amount);

        var response = await mediator.Send(command);

        Assert.NotNull(response);
        Assert.Equal(originId - amount, response.GetResponseObject().Origin.Balance);        
        Assert.Equal(destinationId + amount, response.GetResponseObject().Destination.Balance);
    }

    //# Transfer from non-existing account
    [Theory]
    [InlineData(5000, 90, 30)]
    public async Task RunTransfer_NonExistingOrigin(int originId, int destinationId, double amount)
    {
        TransferCommand command = new(originId, destinationId, amount);

        await Assert.ThrowsAsync<ObjectNotFoundException>(async () => await mediator.Send(command));
    }
    //# Transfer to non-existing account
    [Theory]
    [InlineData(70, 5000, 30)]
    public async Task RunTransfer_NonExistingDestination(int originId, int destinationId, double amount)
    {
        TransferCommand command = new( originId, destinationId, amount);

        await Assert.ThrowsAsync<ObjectNotFoundException>(async () => await mediator.Send(command));
    }

    //# Transfer with insuficient funds
    [Theory]
    [InlineData(5, 70, 30)]
    public async Task RunTransfer_InsuficientFunds(int originId, int destinationId, double amount)
    {
        TransferCommand command = new( originId, destinationId, amount);

        await Assert.ThrowsAsync<BussinessRuleException>(async () => await mediator.Send(command));
    }
}
