using Luciano.Serafim.Banking.Core.Tests.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Luciano.Serafim.Banking.Core.UseCases.Events.Requests;

namespace Luciano.Serafim.Banking.Core.Tests.Core.UseCases.Events;

public class RunDepositUseCaseTests
{
    private readonly IMediator mediator;

    public RunDepositUseCaseTests()
    {
        var services = new ServiceCollection();
        var serviceProvider = services
            .AddTest()
            .BuildServiceProvider();

        mediator = serviceProvider.GetRequiredService<IMediator>();
    }

    //# Create account with initial balance
    [Theory]
    [InlineData(100, 10)]
    public async Task RunDeposit_CreateAccount(int destinationId, double amount)
    {
        DepositCommand command = new(destinationId, amount);

        var response = await mediator.Send(command);

        Assert.NotNull(response);
        Assert.Equal(amount, response.GetResponseObject().Destination.Balance);
    }

    //# Deposit into existing account
    [Theory]
    [InlineData(50, 10)]
    public async Task RunDeposit_ExistingAccount(int destinationId, double amount)
    {
        DepositCommand command = new(destinationId, amount);

        var response = await mediator.Send(command);

        Assert.NotNull(response);
        Assert.Equal(destinationId + amount, response.GetResponseObject().Destination.Balance);
    }

}
