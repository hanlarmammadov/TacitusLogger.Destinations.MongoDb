using MongoDB.Bson;
using System;
using TacitusLogger.Serializers;

namespace TacitusLogger.Destinations.MongoDb
{
    /// <summary>
    /// Implementation of <c>IBsonDocumentBuilder</c> which uses converter delegate of
    /// type <c>LogModelFunc<BsonDocument></c> to construct the BSON document.
    /// </summary>
    public class ConverterFunctionBsonDocumentBuilder : SynchronousBsonDocumentBuilder
    { 
        private readonly LogModelFunc<BsonDocument> _converter;

        /// <summary>
        /// Initializes the instance of <c>ConverterFunctionBsonDocumentBuilder</c> using 
        /// converter delegate of type <c>LogModelFunc<BsonDocument></c>
        /// </summary>
        /// <param name="converter">Converter method.</param>
        public ConverterFunctionBsonDocumentBuilder(LogModelFunc<BsonDocument> converter)
        {
            _converter = converter ?? throw new ArgumentNullException("converter");
        }

        /// <summary>
        /// Gets factory method that was set during the initialization. For tests purposes.
        /// </summary>
        internal LogModelFunc<BsonDocument> Converter => _converter;

        /// <summary>
        /// Constructs object of type <c>BsonDocument</c> using log model.
        /// </summary>
        /// <param name="logModel">Log model</param>
        /// <returns>Created BSON document.</returns>
        public override BsonDocument BuildFrom(LogModel logModel)
        {
            return _converter(logModel);
        }
    }
}
