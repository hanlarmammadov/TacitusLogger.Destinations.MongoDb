using MongoDB.Bson;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TacitusLogger.Destinations.MongoDb
{
    public interface IBsonDocumentBuilder : IDisposable
    {
        BsonDocument BuildFrom(LogModel logModel);
        Task<BsonDocument> BuildFromAsync(LogModel logModel, CancellationToken cancellationToken = default(CancellationToken));
    }
}
