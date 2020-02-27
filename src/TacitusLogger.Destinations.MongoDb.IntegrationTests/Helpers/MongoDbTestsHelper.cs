using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using TacitusLogger.Tests.Helpers;

namespace TacitusLogger.Destinations.MongoDb.IntegrationTests.Helpers
{
    public class MongoDbTestsHelper
    {
        private static IConfigurationRoot _configs;
        private static MongoClient _mongoClient;
        private static IMongoDatabase _mongoDatabase;

        static MongoDbTestsHelper()
        {
            _configs = new ConfigurationBuilder()
                                    .AddJsonFile(".//mongodb-configs.json")
                                    .Build();
            _mongoClient = new MongoClient(_configs["mongoConnString"]);
            _mongoDatabase = _mongoClient.GetDatabase(_configs["mongoDatabase"]);
        }

        public IConfigurationRoot Configs => _configs;
        public MongoClient MongoClient => _mongoClient;
        public IMongoDatabase MongoDatabase => _mongoDatabase;

        public IMongoCollection<T> GetArbitraryNamedMongoCollection<T>()
        {
            string randCollectionName = Samples.Strings.RandomGuidBased();
            return _mongoDatabase.GetCollection<T>(randCollectionName);
        }
    }
}
