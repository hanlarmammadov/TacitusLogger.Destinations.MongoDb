using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace TacitusLogger.Destinations.MongoDb
{
    /// <summary>
    /// Implementation of <c>IMongoCollectionProvider</c> that always returns the same collection 
    /// which it was provided with during initialization.
    /// </summary>
    public class DirectCollectionProvider : SynchronousCollectionProviderBase
    {
        private readonly IMongoCollection<BsonDocument> _mongoCollection;
        
        /// <summary>
        /// Creates an instance of <c>DirectCollectionProvider</c> using MongoDb collection.
        /// </summary>
        /// <exception cref="ArgumentNullException">If <paramref name="mongoCollection"/> is null.</exception>
        /// <param name="mongoCollection">Target collection.</param>
        public DirectCollectionProvider(IMongoCollection<BsonDocument> mongoCollection)
        {
            _mongoCollection = mongoCollection ?? throw new ArgumentNullException("mongoCollection");
        }

        /// <summary>
        /// Gets internal collection. For test purposes.
        /// </summary>
        internal IMongoCollection<BsonDocument> Collection => _mongoCollection;

        /// <summary>
        /// Retrieves the according collection using input data.
        /// </summary>
        /// <param name="logModel"></param>
        /// <returns>Resulting Mongo collection.</returns>
        public override IMongoCollection<BsonDocument> GetCollection(LogModel logModel)
        {
            return _mongoCollection;
        }
    }
}
