using Luciano.Serafim.Banking.Core.Models.Exceptions;
using Luciano.Serafim.Banking.Core.Tests.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Luciano.Serafim.Banking.Core.UseCases.Events.Requests;

namespace Luciano.Serafim.Banking.Core.Tests.Core.UseCases.Events;

public class RunWithdrawUseCaseTests
{
    private readonly IMediator mediator;

    public RunWithdrawUseCaseTests()
    {
        var services = new ServiceCollection();
        var serviceProvider = services
            .AddTest()
            .BuildServiceProvider();

        mediator = serviceProvider.GetRequiredService<IMediator>();
    }

    //# Withdraw from existing account
    [Theory]
    [InlineData(50, 30)]
    public async Task RunWithdraw_Success(int originId, double amount)
    {
        WithdrawCommand command = new(originId, amount );

        var response = await mediator.Send(command);

        Assert.NotNull(response);
        Assert.Equal(originId - amount, response.GetResponseObject().Origin.Balance);
    }

    //# Withdraw from non-existing account
    [Theory]
    [InlineData(5000, 30)]
    public async Task RunWithdraw_NonExistingOrigin(int originId, double amount)
    {
        WithdrawCommand command = new(originId, amount );

        await Assert.ThrowsAsync<ObjectNotFoundException>(async () => await mediator.Send(command));
    }
    
    //# Withdraw with insuficient funds
    [Theory]
    [InlineData(5, 30)]
    public async Task RunWithdraw_InsuficientFunds(int originId, double amount)
    {
        WithdrawCommand command = new( originId, amount );

        await Assert.ThrowsAsync<BussinessRuleException>(async () => await mediator.Send(command));
    }
}
