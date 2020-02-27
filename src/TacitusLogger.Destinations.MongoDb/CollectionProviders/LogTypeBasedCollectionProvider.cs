using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TacitusLogger.Destinations.MongoDb
{
    /// <summary>
    /// Implementation of <c>IMongoCollectionProvider</c> that selects a collection 
    /// based on log types using provided dictionary.
    /// </summary>
    public class LogTypeBasedCollectionProvider : SynchronousCollectionProviderBase
    {
        private readonly Dictionary<LogType, IMongoCollection<BsonDocument>> _dictionary;

        /// <summary>
        /// Creates an instance of <c>LogTypeBasedCollectionProvider</c> using dictionary.
        /// </summary>
        /// <exception cref="ArgumentNullException">If <paramref name="dictionary"/> is null.</exception>
        /// <exception cref="ArgumentException">If dictionary contains not all log types or dictionary contains null values.</exception>
        /// <param name="dictionary">Log type - Mongo collection dictionary.</param>
        public LogTypeBasedCollectionProvider(Dictionary<LogType, IMongoCollection<BsonDocument>> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");
            if (dictionary.Count != Enum.GetNames(typeof(LogType)).Length)
                throw new ArgumentException("Dictionary contains key-value pairs not for all log types");
            if (dictionary.ContainsValue(null))
                throw new ArgumentException("Dictionary contains null values");

            _dictionary = dictionary;
        }

        /// <summary>
        /// Gets the dictionary that was specified during the initialization.
        /// </summary>
        public Dictionary<LogType, IMongoCollection<BsonDocument>> Dictionary => _dictionary;

        /// <summary>
        /// Retrives the according collection using input data.
        /// </summary>
        /// <param name="logModel"></param>
        /// <returns>Resulting Mongo collection.</returns>
        public override IMongoCollection<BsonDocument> GetCollection(LogModel logModel)
        {
            if (logModel == null)
                throw new ArgumentNullException("logModel");

            return _dictionary[logModel.LogType];
        }
    }
}
