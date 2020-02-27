using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TacitusLogger.Destinations.MongoDb
{
    /// <summary>
    /// Convenient to be inherited from if the collection selection operation represents a quick synchronous process
    /// which does not assume any specific overriding of the async counterpart of GetCollection method.
    /// </summary>
    public abstract class SynchronousCollectionProviderBase : IMongoCollectionProvider
    {
        /// <summary>
        /// Should be overridden in subclasses.
        /// </summary>
        /// <param name="logModel"></param>
        /// <returns></returns>
        public abstract IMongoCollection<BsonDocument> GetCollection(LogModel logModel);
        /// <summary>
        /// Asynchronous counterpart of GetCollection method.
        /// </summary>
        /// <param name="logModel">Log model.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The value of the TResult represents the collection.</returns>
        public Task<IMongoCollection<BsonDocument>> GetCollectionAsync(LogModel logModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<IMongoCollection<BsonDocument>>(cancellationToken);
            return Task.FromResult(GetCollection(logModel));
        }
        public void Dispose()
        {

        }
    }
}
