using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using TacitusLogger.Destinations.MongoDb.BsonDocumentBuilders;

namespace TacitusLogger.Destinations.MongoDb
{
    /// <summary>
    /// Implementation of <c>IBsonDocumentBuilder</c> which builds BSON document from log model using <c>TacitusLogger.Destinations.MongoDb.BsonDocumentBuilders.BsonSerializableLogModel</c>.
    /// </summary>
    public class DirectBsonDocumentBuilder : SynchronousBsonDocumentBuilder
    {
        /// <summary>
        /// Constructs object of type <c>BsonDocument</c> using log model of type <c>TacitusLogger.Destinations.MongoDb.BsonDocumentBuilders.BsonSerializableLogModel</c>.
        /// </summary>
        /// <param name="logModel">Log model.</param>
        /// <returns>Created BSON document.</returns>
        public override BsonDocument BuildFrom(LogModel logModel)
        {
            if (logModel == null)
                throw new ArgumentNullException("logModel");
            return new BsonSerializableLogModel(logModel).ToBsonDocument();
        }
    }
}
