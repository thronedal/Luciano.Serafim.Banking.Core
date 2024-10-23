using System.Reflection;
using Luciano.Serafim.Banking.Core.Bootstrap;
using Luciano.Serafim.Banking.Core.Abstractions.Transactions;
using Luciano.Serafim.Banking.Core.Models;
using Medallion.Threading;
using Medallion.Threading.FileSystem;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Luciano.Serafim.Banking.Core.Abstractions.Repositories;

namespace Luciano.Serafim.Banking.Core.Tests.Utility;

public static class TestHelper
{
    public static IServiceCollection AddTest(this IServiceCollection services)
    {
        services
            .AddMediatRConfiguration(Assembly.Load("Luciano.Serafim.Banking.Core.Tests"))
            .AddLogging()
            .AddDistributedMemoryCache()
            .AddResponse()
            .AddAccountService()
            .AddEventService()
            .AddDistributedLockTest()
            .AddScoped(_=> Substitute.For<IUnitOfWork>());

        return services;
    }

    public static IServiceCollection AddAccountService(this IServiceCollection services)
    {
        List<Account> accounts = new();
        List<AccountConsolidatedBalance> consolidatedBalances = new();

        var initializeAccounts = () =>
        {
            accounts.Clear();
            accounts.AddRange(Enumerable.Range(1, 99).Select(i => new Account(i, i.ToString())).ToList());
            accounts.Add(new Account(300, "300"));

            consolidatedBalances.AddRange(accounts.Select(a => new AccountConsolidatedBalance(a, DateOnly.FromDateTime(DateTime.UtcNow), a.Id)));
        };

        initializeAccounts();

        var createAccount = (Account account) =>
        {
            accounts.Add(account);

            return account;
        };     

        var consolidateBalance = (Account account, DateOnly date, double balance) =>
        {
            var consolidated = new AccountConsolidatedBalance(account, DateOnly.FromDateTime(DateTime.UtcNow), balance);
            consolidatedBalances.Add(consolidated);

            return consolidated;
        };     

        services.AddScoped(_ =>
                    {
                        var service = Substitute.For<IAccountRepository>();

                        //account exists
                        service.GetAccountById(Arg.Any<int>()).Returns(call => accounts.Where(a => a.Id == call.ArgAt<int>(0)).FirstOrDefault());

                        //return the last consolidated balance, or a consolidate data of DateOnly.MinValue and Balance 0 if there none 
                        service.GetLastConsolidatedBalance(Arg.Any<int>()).Returns(call => 
                            consolidatedBalances.Where(c => c.Account.Id == call.ArgAt<int>(0) 
                                                        && c.BalanceDate == consolidatedBalances.Where(c => c.Account.Id == call.ArgAt<int>(0)).Max(m => m.BalanceDate))
                                .FirstOrDefault(new AccountConsolidatedBalance(new Account(call.ArgAt<int>(0), call.ArgAt<int>(0).ToString()), DateOnly.MinValue, 0))
                            );

                        //create account
                        service.CreateAccount(Arg.Any<Account>()).Returns(call => createAccount(call.ArgAt<Account>(0)));

                        //consolidate balance
                        service.ConsolidateBalance(Arg.Any<Account>(),Arg.Any<DateOnly>(),Arg.Any<double>()).Returns(call => consolidateBalance(call.ArgAt<Account>(0), call.ArgAt<DateOnly>(1),call.ArgAt<double>(2)));

                        //AccountExists
                        service.AccountExists(Arg.Any<int>()).Returns(call => accounts.Exists(a => a.Id == call.ArgAt<int>(0)));

                        return service;
                    });

        return services;
    }

    public static IServiceCollection AddEventService(this IServiceCollection services)
    {
        List<Event> events = new();

        var createEvent = (Event @event) =>
        {
            var e = new Event(@event.Operation, @event.Amount, DateTime.UtcNow, @event.AccountId) { Id = Guid.NewGuid().ToString() };
            events.Add(e);

            return e;
        };

        var getEventsAfter = (int accountId, DateTime initialDate) => events.Where(e => e.AccountId == accountId && e.Ocurrence >= initialDate).ToList();

        services
            .AddScoped(_ =>
            {
                var service = Substitute.For<IEventRepository>();

                //returns a empty list of events
                service.GetEventsAfter(Arg.Any<int>(), Arg.Any<DateTime>()).Returns(call => getEventsAfter(call.ArgAt<int>(0), call.ArgAt<DateTime>(1)));
                service.CreateEvent(Arg.Any<Event>()).Returns(call => createEvent(call.ArgAt<Event>(0)));

                return service;
            });

        return services;
    }

    
    public static IServiceCollection AddDistributedLockTest(this IServiceCollection services)
    {
        //for testing purposes, file lock is configured
        services.AddSingleton<IDistributedLockProvider>(_ => new FileDistributedSynchronizationProvider(new DirectoryInfo(Environment.CurrentDirectory)));

        return services;
    }
}