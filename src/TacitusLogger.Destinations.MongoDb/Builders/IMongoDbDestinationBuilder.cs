using TacitusLogger.Builders;

namespace TacitusLogger.Destinations.MongoDb
{
    public interface IMongoDbDestinationBuilder : IDestinationBuilder
    {
        IMongoDbDestinationBuilder WithCollection(IMongoCollectionProvider collectionProvider);
        IMongoDbDestinationBuilder WithBsonDocument(IBsonDocumentBuilder bsonDocumentBuilder);
    }
}
