using System;
using Luciano.Serafim.Banking.Core.Abstractions.Locking;
using Luciano.Serafim.Banking.Core.Models.Exceptions;
using Luciano.Serafim.Banking.Core.Models;
using Luciano.Serafim.Banking.Core.Tests.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Luciano.Serafim.Banking.Core.Tests.Core.MediatR;

public class LockingBehaviourTests
{

    private readonly IMediator mediator;

    public LockingBehaviourTests()
    {
        var services = new ServiceCollection();
        var serviceProvider = services
            .AddTest()
            .BuildServiceProvider();

        mediator = serviceProvider.GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task AcquireSingleLock_ResourceLocked()
    {
        var tasks = new List<Task>();
        //locks the resource
        tasks.Add(mediator.Send(new LockResourceCommand() { Id = 1, Value = 15 }));

        //try to use the locked resource
        tasks.Add(mediator.Send(new LockResourceCommand() { Id = 1, Value = 20 }));

        var ex = Assert.Throws<AggregateException>(() => Task.WaitAll(tasks.ToArray()));
        Assert.IsType<ResourceLockingTimeOutException>(ex.InnerException);
    }

    [Fact]
    public async Task AcquireSingleLock_WaitSucess()
    {
        var tasks = new List<Task>();
        //locks the resource
        tasks.Add(mediator.Send(new LockResourceCommand() { Id = 1, Value = 3 }));

        //try to use the locked resource
        tasks.Add(mediator.Send(new LockResourceCommand() { Id = 1, Value = 5 }));

        Task.WaitAll(tasks.ToArray());
    }

    [Fact]
    public async Task AcquireSingleLock_Sucess()
    {
        var tasks = new List<Task>();
        //locks the resource
        tasks.Add(mediator.Send(new LockResourceCommand() { Id = 1, Value = 3 }));

        //try to use the locked resource
        tasks.Add(mediator.Send(new LockResourceCommand() { Id = 2, Value = 5 }));

        Task.WaitAll(tasks.ToArray());
    }

    [Fact]
    public async Task AcquireMultipleLock_ResourceLocked()
    {
        var tasks = new List<Task>();
        //locks the resource
        tasks.Add(mediator.Send(new LockmultipleResourceCommand() { Id = 1, Id2 = 3, Value = 15 }));

        //try to use the locked resource
        tasks.Add(mediator.Send(new LockmultipleResourceCommand() { Id = 1, Id2 = 3, Value = 20 }));

        var ex = Assert.Throws<AggregateException>(() => Task.WaitAll(tasks.ToArray()));
        Assert.IsType<ResourceLockingTimeOutException>(ex.InnerException);
    }

    [Fact]
    public async Task AcquiremultipleLock_WaitSucess()
    {
        var tasks = new List<Task>();
        //locks the resource
        tasks.Add(mediator.Send(new LockmultipleResourceCommand() { Id = 1, Id2 = 3, Value = 3 }));

        //try to use the locked resource
        tasks.Add(mediator.Send(new LockmultipleResourceCommand() { Id = 1, Id2 = 3, Value = 5 }));

        Task.WaitAll(tasks.ToArray());
    }

    [Fact]
    public async Task AcquireMultipleLock_Sucess()
    {
        var tasks = new List<Task>();
        //locks the resource
        tasks.Add(mediator.Send(new LockmultipleResourceCommand() { Id = 1, Id2 = 3, Value = 3 }));

        //try to use the locked resource
        tasks.Add(mediator.Send(new LockmultipleResourceCommand() { Id = 2, Id2 = 4, Value = 5 }));

        Task.WaitAll(tasks.ToArray());
    }

}

/// <summary>
/// Test Command utilizing the ResourceLocking behaviour
/// To utilize cache its only need do implements ICacheable Interface
/// </summary>
public class LockResourceCommand : IRequest<Response<int>>, IResourceLocking
{
    public int Id { get; set; }
    public int Value { get; set; }
    public string Resources => $"Resource_{Id}";

    public TimeSpan TimeOut { get; } = TimeSpan.FromSeconds(5);
}

public class LockingBehaviourTestUseCase : IRequestHandler<LockResourceCommand, Response<int>>
{
    private readonly Response<int> response;

    public LockingBehaviourTestUseCase(Response<int> response)
    {
        this.response = response;
    }
    public async Task<Response<int>> Handle(LockResourceCommand request, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(request.Value));
        response.SetResponsePayload(request.Value);
        return await Task.FromResult(response);
    }
}

/// <summary>
/// Test Command utilizing the ResourceLocking behaviour
/// To utilize cache its only need do implements ICacheable Interface
/// </summary>
public class LockmultipleResourceCommand : IRequest<Response<int>>, IResourceLocking
{
    public int Id { get; set; }
    public int Id2 { get; set; }
    public int Value { get; set; }
    public string Resources => $"Resource_{Id},Resource_{Id2}";

    public TimeSpan TimeOut { get; } = TimeSpan.FromSeconds(5);
}

public class LockingMultipleBehaviourTestUseCase : IRequestHandler<LockmultipleResourceCommand, Response<int>>
{
    private readonly Response<int> response;

    public LockingMultipleBehaviourTestUseCase(Response<int> response)
    {
        this.response = response;
    }
    public async Task<Response<int>> Handle(LockmultipleResourceCommand request, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(request.Value));
        response.SetResponsePayload(request.Value);
        return await Task.FromResult(response);
    }
}