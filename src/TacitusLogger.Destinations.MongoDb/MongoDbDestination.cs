using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TacitusLogger.Components.Helpers;
using TacitusLogger.Destinations;

namespace TacitusLogger.Destinations.MongoDb
{
    /// <summary>
    /// Destination that writes log information to MongoDb database.
    /// </summary>
    public class MongoDbDestination : ILogDestination
    {
        private readonly IMongoCollectionProvider _mongoCollectionProvider;
        private readonly IBsonDocumentBuilder _bsonDocumentBuilder;

        /// <summary>
        /// Creates an instance of <c>MongoDbDestination</c> class using mongo collection provider of type <c>IMongoCollectionProvider</c>
        /// and BSON builder of type <c>IBsonDocumentBuilder.</c>
        /// </summary>
        /// <param name="mongoCollectionProvider">MongoDb collection provider.</param>
        /// <param name="bsonDocumentBuilder">BSON builder.</param>
        public MongoDbDestination(IMongoCollectionProvider mongoCollectionProvider, IBsonDocumentBuilder bsonDocumentBuilder)
        {
            _mongoCollectionProvider = mongoCollectionProvider ?? throw new ArgumentNullException("mongoCollectionProvider");
            _bsonDocumentBuilder = bsonDocumentBuilder ?? throw new ArgumentNullException("bsonDocumentBuilder");
        }

        /// <summary>
        /// Gets Mongo collection provider specified during initialization.
        /// </summary>
        public IMongoCollectionProvider CollectionProvider => _mongoCollectionProvider;
        /// <summary>
        /// Gets BSON builder specified during initialization.
        /// </summary>
        public IBsonDocumentBuilder BsonDocumentBuilder => _bsonDocumentBuilder;

        /// <summary>
        /// Writes log models to the destination.
        /// </summary>
        /// <param name="logs">Log models array.</param>
        public void Send(LogModel[] logs)
        {
            if (logs.Length == 1)
            {
                // Generate BSON document.
                BsonDocument doc = _bsonDocumentBuilder.BuildFrom(logs[0]) ?? throw new Exception("Bson builder returned null");
                // Retrieve the according mongo collection.
                IMongoCollection<BsonDocument> mongoColl = _mongoCollectionProvider.GetCollection(logs[0]) ?? throw new Exception("Mongo collection provider returned null");
                // Save the BSON document.
                mongoColl.InsertOne(doc);
            }
            else
            {
                // Use caching.
                var dict = new Dictionary<IMongoCollection<BsonDocument>, IList<BsonDocument>>();
                for (int i = 0; i < logs.Length; i++)
                {
                    // Retrieve the according mongo collection.
                    IMongoCollection<BsonDocument> mongoColl = _mongoCollectionProvider.GetCollection(logs[i]) ?? throw new Exception("Mongo collection provider returned null");
                    // Add to dictionary if it is not there already.
                    dict.TryAdd(mongoColl, new List<BsonDocument>());
                    // Generate BSON document and add to the according dictionary key.
                    dict[mongoColl].Add(_bsonDocumentBuilder.BuildFrom(logs[i]) ?? throw new Exception("Bson builder returned null"));
                }
                // Insert BSON documents into db.
                foreach (var keyValuePair in dict)
                {
                    if (keyValuePair.Value.Count == 1)
                        keyValuePair.Key.InsertOne(keyValuePair.Value[0]);
                    else
                        keyValuePair.Key.InsertMany(keyValuePair.Value);
                }
            }
        }
        /// <summary>
        /// Asynchronously writes log models to the destination.
        /// </summary>
        /// <param name="logs">Log models array.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SendAsync(LogModel[] logs, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (logs.Length == 1)
            {
                // Generate BSON document.
                BsonDocument doc = await _bsonDocumentBuilder.BuildFromAsync(logs[0]) ?? throw new Exception("Bson builder returned null");
                // Retrieve the according mongo collection.
                IMongoCollection<BsonDocument> mongoColl = await _mongoCollectionProvider.GetCollectionAsync(logs[0]) ?? throw new Exception("Mongo collection provider returned null");
                // Save the BSON document.
                await mongoColl.InsertOneAsync(doc);
            }
            else
            {
                // Use caching.
                var dict = new Dictionary<IMongoCollection<BsonDocument>, IList<BsonDocument>>();
                for (int i = 0; i < logs.Length; i++)
                {
                    // Retrieve the according mongo collection.
                    IMongoCollection<BsonDocument> mongoColl = await _mongoCollectionProvider.GetCollectionAsync(logs[i]) ?? throw new Exception("Mongo collection provider returned null");
                    // Add to dictionary if it is not there already.
                    dict.TryAdd(mongoColl, new List<BsonDocument>());
                    // Generate BSON document and add to the according dictionary key.
                    dict[mongoColl].Add(await _bsonDocumentBuilder.BuildFromAsync(logs[i]) ?? throw new Exception("Bson builder returned null"));
                }
                // Insert BSON documents into db.
                foreach (var keyValuePair in dict)
                {
                    if (keyValuePair.Value.Count == 1)
                        await keyValuePair.Key.InsertOneAsync(keyValuePair.Value[0]);
                    else
                        await keyValuePair.Key.InsertManyAsync(keyValuePair.Value);
                }
            }
        }
        public void Dispose()
        {
            try
            {
                _mongoCollectionProvider.Dispose();
            }
            catch { }
            try
            {
                _bsonDocumentBuilder.Dispose();
            }
            catch { }
        }
    }
}
