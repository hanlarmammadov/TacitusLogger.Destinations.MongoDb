using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace TacitusLogger.Destinations.MongoDb
{
    /// <summary>
    /// Convenient to be inherited from if the BSON document building operation represents a quick synchronous process
    /// which does not assume any specific overriding of the async counterpart of BuildFrom method.
    /// </summary>
    public abstract class SynchronousBsonDocumentBuilder : IBsonDocumentBuilder
    {
        /// <summary>
        /// Should be overridden in subclasses.
        /// </summary>
        /// <param name="logModel">Log model.</param>
        /// <returns></returns>
        public abstract BsonDocument BuildFrom(LogModel logModel);
        /// <summary>
        /// Asynchronous counterpart of BuildFrom method.
        /// </summary>
        /// <param name="logModel">Log model.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The value of the TResult represents the BSON document.</returns>
        public Task<BsonDocument> BuildFromAsync(LogModel logModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<BsonDocument>(cancellationToken);
            return Task.FromResult(BuildFrom(logModel));
        }
        public virtual void Dispose()
        {

        }
    }
}
