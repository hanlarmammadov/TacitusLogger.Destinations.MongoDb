using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

namespace TacitusLogger.Destinations.MongoDb
{
    public static class IMongoDbDestinationBuilderExtensions
    {
        #region Extension methods related to WithCollection method

        public static IMongoDbDestinationBuilder WithCollection(this IMongoDbDestinationBuilder self, IMongoCollection<BsonDocument> mongoCollection)
        {
            return self.WithCollection(new DirectCollectionProvider(mongoCollection));
        }
        public static IMongoDbDestinationBuilder WithCollection(this IMongoDbDestinationBuilder self, LogModelFunc<IMongoCollection<BsonDocument>> generatorFunction)
        {
            return self.WithCollection(new GeneratorFunctionCollectionProvider(generatorFunction));
        }
        public static IMongoDbDestinationBuilder WithCollection(this IMongoDbDestinationBuilder self, IMongoDatabase mongoDatabase, string collectionNameTemplate)
        {
            return self.WithCollection(new NameTemplateCollectionProvider(mongoDatabase, collectionNameTemplate));
        }
        public static IMongoDbDestinationBuilder WithCollection(this IMongoDbDestinationBuilder self, Dictionary<LogType, IMongoCollection<BsonDocument>> dictionary)
        {
            return self.WithCollection(new LogTypeBasedCollectionProvider(dictionary));
        }

        #endregion

        #region Extension methods related to WithBsonDocument method

        public static IMongoDbDestinationBuilder WithLogModelBsonDocument(this IMongoDbDestinationBuilder self)
        {
            return self.WithBsonDocument(new DirectBsonDocumentBuilder());
        } 
        public static IMongoDbDestinationBuilder WithBsonDocument(this IMongoDbDestinationBuilder self, LogModelFunc<BsonDocument> generatorFunction)
        {
            return self.WithBsonDocument(new ConverterFunctionBsonDocumentBuilder(generatorFunction));
        }

        #endregion
    }
}
