using Luciano.Serafim.Banking.Core.Models.Exceptions;
using Luciano.Serafim.Banking.Core.Tests.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Luciano.Serafim.Banking.Core.UseCases.Accounts.Requests;

namespace Luciano.Serafim.Banking.Core.Tests.Core.UseCases.Accounts;

public class GetBalanceUseCaseTests
{
    private readonly IMediator mediator;

    public GetBalanceUseCaseTests()
    {
        var services = new ServiceCollection();
        var serviceProvider = services
            .AddTest()
            .BuildServiceProvider();

        mediator = serviceProvider.GetRequiredService<IMediator>();
    }


    //# Get balance for non-existing account
    [Fact]
    public async Task GetBalance_NonExistingAccount()
    {
        GetBalanceQuery query = new( 1100 );

        await Assert.ThrowsAsync<ObjectNotFoundException>(async () => await mediator.Send(query));
    }

    //# Get balance for existing account
    [Theory]
    [InlineData(1, 1)]
    [InlineData(30, 30)]
    [InlineData(87, 87)]
    public async Task GetBalance_ExistingAccount(int accountId, double balance)
    {
        GetBalanceQuery query = new( accountId );

        var response = await mediator.Send(query);

        Assert.NotNull(response);
        Assert.Equal(balance, response.GetResponseObject());
    }
}
