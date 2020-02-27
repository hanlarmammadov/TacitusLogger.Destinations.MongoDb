using MongoDB.Driver;
using NUnit.Framework;
using System.Linq;
using MongoDB.Bson;
using TacitusLogger.Tests.Helpers;
using TacitusLogger.Destinations.MongoDb.IntegrationTests.Helpers;

namespace TacitusLogger.Destinations.MongoDb.IntegrationTests
{
    [TestFixture]
    public class MongoDbDriverLibraryTests
    {
        private static readonly object _tearDownLock = new object();
        private MongoDbTestsHelper _mongoDbTestsHelper;

        public MongoDbDriverLibraryTests()
        {
            _mongoDbTestsHelper = new MongoDbTestsHelper();
        }

        [TearDown]
        public void CleanMongoDb()
        {
            //lock (_tearDownLock)
            //{
            //    var mongodb = _mongoDbTestsHelper.MongoDatabase;
            //    foreach (var collName in mongodb.ListCollectionNames().ToList())
            //        mongodb.DropCollection(collName);
            //}
        }

        [Test]
        public void Bson_Representation_Of_LogData_Should_Not_Contain_Discriminator()
        {
            //Arrange
            LogModel logModel = Samples.LogModels.Standard();

            //Act
            BsonDocument bsonDoc = logModel.ToBsonDocument(typeof(LogModel));

            //Assert
            string resultJsonStr = bsonDoc.ToJson();
            string logModelJsonString = logModel.ToJson();

            Assert.AreEqual(logModelJsonString, resultJsonStr);
        }
    }
}
