using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TacitusLogger.Destinations.MongoDb
{
    public interface IMongoCollectionProvider : IDisposable
    {
        IMongoCollection<BsonDocument> GetCollection(LogModel logModel);
        Task<IMongoCollection<BsonDocument>> GetCollectionAsync(LogModel logModel, CancellationToken cancellationToken = default(CancellationToken));
    }
}
