using Luciano.Serafim.Banking.Core.Models;
using MongoDB.Driver;
using Luciano.Serafim.Banking.Core.Abstractions.Transactions;
using Luciano.Serafim.Banking.Core.Abstractions.Repositories;

namespace Luciano.Serafim.Banking.Core.Infrastructure.MongoDb;

public class EventRepository : IEventRepository
{
    private IMongoCollection<Event> eventsCollection;
    private readonly IUnitOfWork unitOfWork;

    public EventRepository(IMongoClient mongoClient, IUnitOfWork unitOfWork)
    {
        var mongoDatabase = mongoClient.GetDatabase(Utils.ACCOUNT_DATABASE_NAME);
        eventsCollection = mongoDatabase.GetCollection<Event>(Utils.EVENT_COLLECTION_NAME);
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc/>
    public async Task<bool> InitializeState()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task<Event> CreateEvent(Event @event)
    {
        await eventsCollection.InsertOneAsync(unitOfWork.GetSession<IClientSessionHandle>(), @event);
        return @event;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Event>> GetEventsAfter(int accountId, DateTime initialDate)
    {
        return await eventsCollection.Find(unitOfWork.GetSession<IClientSessionHandle>(), e => e.AccountId == accountId && e.Ocurrence >= initialDate).ToListAsync();
    }
}

