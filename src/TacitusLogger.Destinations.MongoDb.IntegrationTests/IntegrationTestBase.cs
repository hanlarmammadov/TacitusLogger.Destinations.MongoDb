//using Microsoft.Extensions.Configuration;
//using MongoDB.Driver; 
//using TacitusLogger.Tests.Helpers;

//namespace TacitusLogger.Destinations.MongoDb.IntegrationTests
//{
//    public abstract class IntegrationTestBase
//    {
//        protected static IConfigurationRoot _configs;
//        private static MongoClient _mongoClient;
//        private static IMongoDatabase _mongoDatabase;
//        private static readonly object _mongoClientLock = new object();
//        private static readonly object _mongoDatabaseLock = new object();
//        private static readonly object _configsLock = new object();

//        protected IConfigurationRoot Configs
//        {
//            get
//            {
//                if (_configs == null)
//                    lock (_configsLock)
//                        if (_configs == null)
//                            _configs = new ConfigurationBuilder()
//                                      .AddJsonFile(".//mongodb-configs.json")
//                                      .Build();
//                return _configs;
//            }
//        }

//        protected MongoClient TestMongoClient
//        {
//            get
//            {
//                if (_mongoClient == null)
//                    lock (_mongoClientLock)
//                        if (_mongoClient == null)
//                            _mongoClient = new MongoClient(Configs["mongoConnString"]);
//                return _mongoClient;
//            }
//        }
//        protected IMongoDatabase TestMongoDatabase
//        {
//            get
//            {
//                if (_mongoDatabase == null)
//                    lock (_mongoDatabaseLock)
//                        if (_mongoDatabase == null)
//                            _mongoDatabase = TestMongoClient.GetDatabase(Configs["mongoDatabase"]);
//                return _mongoDatabase;
//            }
//        } 
//        protected IMongoCollection<T> GetArbitraryNamedMongoCollection<T>()
//        {
//            string randCollectionName = Samples.Strings.RandomGuidBased();
//            return TestMongoDatabase.GetCollection<T>(randCollectionName);
//        }
//    }
//}
