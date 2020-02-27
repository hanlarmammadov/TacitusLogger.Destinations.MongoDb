using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TacitusLogger.Destinations.MongoDb
{
    /// <summary>
    /// Implementation of <c>IMongoCollectionProvider</c> which uses factory method of
    /// type <c>TacitusLogger.LogModelFunc<IMongoCollection<BsonDocument>></c> to return the right collection.
    /// </summary>
    public class GeneratorFunctionCollectionProvider : SynchronousCollectionProviderBase
    {
        private readonly LogModelFunc<IMongoCollection<BsonDocument>> _generatorFunction;

        /// <summary>
        /// Creates an instance of <c>GeneratorFunctionCollectionProvider</c> class using factory method.
        /// </summary>
        /// <param name="generatorFunction">Factory method of type <c>TacitusLogger.LogModelFunc<IMongoCollection<BsonDocument>></c>.</param>
        public GeneratorFunctionCollectionProvider(LogModelFunc<IMongoCollection<BsonDocument>> generatorFunction)
        {
            _generatorFunction = generatorFunction ?? throw new ArgumentNullException("generatorFunction");
        }

        /// <summary>
        /// Gets the factory method which it was provided with during initialization.
        /// </summary>
        public LogModelFunc<IMongoCollection<BsonDocument>> GeneratorFunction => _generatorFunction;

        /// <summary>
        /// Retrieves the according collection using input data.
        /// </summary>
        /// <param name="logModel"></param> 
        /// <returns>Resulting Mongo collection.</returns>
        public override IMongoCollection<BsonDocument> GetCollection(LogModel logModel)
        {
            return _generatorFunction(logModel);
        } 
    }
}
