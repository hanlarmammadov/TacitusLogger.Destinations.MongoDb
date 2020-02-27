using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TacitusLogger.Builders;
using TacitusLogger.Destinations;
using TacitusLogger.Destinations.MongoDb.IntegrationTests.Helpers;
using TacitusLogger.Tests.Helpers;

namespace TacitusLogger.Destinations.MongoDb.IntegrationTests
{
    [TestFixture]
    public class MongoDbDestinationTests
    {
        private static readonly object _tearDownLock = new object();
        private MongoDbTestsHelper _mongoDbTestsHelper;

        protected void AssertIfBsonDocIsEquvalToLogModel(BsonDocument bsonDoc, LogModel logModel)
        {
            //Assert    
            Assert.AreEqual(9, bsonDoc.Elements.Count());
            bsonDoc.RemoveElement(bsonDoc.GetElement("_id"));
            LogModel logModelFromBson = BsonSerializer.Deserialize<LogModel>(bsonDoc);

            Assert.AreEqual(logModel.LogId, logModelFromBson.LogId);
            Assert.AreEqual(logModel.Context, logModelFromBson.Context);
            Assert.AreEqual(logModel.Source, logModelFromBson.Source);
            Assert.AreEqual(logModel.LogType, logModelFromBson.LogType);
            Assert.AreEqual(logModel.Description, logModelFromBson.Description);
            Assert.AreEqual(logModel.LogDate.ToUniversalTime().ToString("dd.MM.yyyy hh.mm"), logModelFromBson.LogDate.ToString("dd.MM.yyyy hh.mm")); 
            Assert.AreEqual(logModel.LogItems.ToJson(), logModelFromBson.LogItems.ToJson());
        }

        public MongoDbDestinationTests()
        {
            _mongoDbTestsHelper = new MongoDbTestsHelper();
        }

        [TearDown]
        public void CleanMongoDb()
        {
            //lock (_tearDownLock)
            //{
            //    var mongodb = _mongoDbTestsHelper.MongoDatabase;
            //    foreach (var collName in mongodb.ListCollections().ToList().ListCollectionNames().ToList())
            //        mongodb.DropCollection(collName);
            //}
        }

        [Test]
        public void Logger_When_Adding_MongoDbDestination_Builds_Correctly()
        {
            //Arrange 
            var collection = _mongoDbTestsHelper.GetArbitraryNamedMongoCollection<BsonDocument>(); 

            //Act
            Logger logger = (Logger)LoggerBuilder.Logger().ForAllLogs().MongoDb().WithCollection(collection)
                                                                                 .Add()
                                                                                 .BuildLogger();
            //Assert 
            ILogDestination MongoDestination = (logger.LogGroups.Single() as LogGroup).LogDestinations.Single();
            Assert.IsInstanceOf<MongoDbDestination>(MongoDestination);
            Assert.IsInstanceOf<DirectBsonDocumentBuilder>((MongoDestination as MongoDbDestination).BsonDocumentBuilder);
            Assert.IsInstanceOf<DirectCollectionProvider>((MongoDestination as MongoDbDestination).CollectionProvider);
        }

        [Test]
        public void Logger_When_Adding_MongoDbDestination_Without_Specifying_CollectionProvider_Throws_Exception()
        {
            //Arrange
            string collName = Samples.Strings.RandomGuidBased();
            var database = _mongoDbTestsHelper.MongoDatabase;

            Assert.Catch<Exception>(() =>
            {
                //Act
                Logger logger = (Logger)LoggerBuilder.Logger().ForAllLogs()
                                                                  .MongoDb()
                                                                  .Add()
                                                              .BuildLogger();
            });
        }

        [Test]
        public void MongoDb_That_Contains_One_MongoDbDestination_For_All_Tests_Saves_Log_To_Database_Correctly()
        {
            //Arrange
            string collName = Samples.Strings.RandomGuidBased(); 
            var database = _mongoDbTestsHelper.MongoDatabase;
            LogModel expectedLogModel = Samples.LogModels.Standard(); 
            Logger logger = (Logger)LoggerBuilder.Logger(expectedLogModel.Source).ForAllLogs()
                                                                                     .MongoDb()
                                                                                     .WithCollection(database, collName)
                                                                                     .Add()
                                                                                 .BuildLogger();
            //Act
            expectedLogModel.LogId = logger.LogError(expectedLogModel.Context, expectedLogModel.Description, expectedLogModel.LogItems);
            var dbList = database.GetCollection<BsonDocument>(collName).Find(x => true).ToList();

            //Assert
            Assert.AreEqual(1, dbList.Count);
            BsonDocument bsonDoc = dbList.First();
            AssertIfBsonDocIsEquvalToLogModel(bsonDoc, expectedLogModel);
        }

        [Test]
        public async Task MongoDb_That_Contains_One_MongoDbDestination_For_All_Tests_Async_Saves_Log_To_Database_Correctly()
        {
            //Arrange
            string collName = Samples.Strings.RandomGuidBased();
            var database = _mongoDbTestsHelper.MongoDatabase;
            LogModel expectedLogModel = Samples.LogModels.Standard();
            Logger logger = (Logger)LoggerBuilder.Logger(expectedLogModel.Source).ForAllLogs()
                                                                                    .MongoDb()
                                                                                    .WithCollection(database, collName)
                                                                                    .Add()
                                                                                 .BuildLogger();
            //Act
            expectedLogModel.LogId = await logger.LogErrorAsync(expectedLogModel.Context, expectedLogModel.Description, expectedLogModel.LogItems);
            var dbList = database.GetCollection<BsonDocument>(collName).Find(x => true).ToList();

            //Assert
            Assert.AreEqual(1, dbList.Count);
            BsonDocument bsonDoc = dbList.First();
            AssertIfBsonDocIsEquvalToLogModel(bsonDoc, expectedLogModel);
        }

        [Test]
        public void MongoDb_With_Specified_Collection_Saves_Log_To_Database_Correctly()
        {
            //Arrange
            var collection = _mongoDbTestsHelper.GetArbitraryNamedMongoCollection<BsonDocument>();
            LogModel expectedLogModel = Samples.LogModels.Standard();
            Logger logger = (Logger)LoggerBuilder.Logger(expectedLogModel.Source).ForAllLogs()
                                                                                     .MongoDb()
                                                                                     .WithCollection(collection)
                                                                                     .Add()
                                                                                 .BuildLogger();
            //Act
            expectedLogModel.LogId = logger.LogError(expectedLogModel.Context, expectedLogModel.Description, expectedLogModel.LogItems);
            var dbList = collection.Find(x => true).ToList();

            //Assert
            Assert.AreEqual(1, dbList.Count);
            BsonDocument bsonDoc = dbList.First();
            AssertIfBsonDocIsEquvalToLogModel(bsonDoc, expectedLogModel);
        }

        [Test]
        public void MongoDb_With_Specified_Collection_Name_Template_Saves_Logs_To_Collections_Correctly()
        {
            //Arrange
            var database = _mongoDbTestsHelper.MongoDatabase;
            Logger logger = (Logger)LoggerBuilder.Logger("App1").ForAllLogs().MongoDb()
                                                                             .WithCollection(database, "$Source_$Context_$LogType_$LogDate(dd-MM-yyyy hh)")
                                                                             .Add()
                                                                             .BuildLogger();
            //Act
            var infoLogId = logger.LogInfo("Context1", "Description1");
            var warningLogId = logger.LogWarning("Context2", "Description1");


            //Assert 
            var infoCollection = database.GetCollection<BsonDocument>($"App1_Context1_Info_{DateTime.Now.ToString("dd-MM-yyyy hh")}");
            var warningCollection = database.GetCollection<BsonDocument>($"App1_Context2_Warning_{DateTime.Now.ToString("dd-MM-yyyy hh")}");
            BsonDocument infoLogBson = infoCollection.Find(x => true).ToList().Single();
            BsonDocument warningLogBson = warningCollection.Find(x => true).ToList().Single();
            Assert.AreNotEqual(infoLogId, warningLogId);
            Assert.AreEqual(infoLogId, infoLogBson.GetElement("LogId").Value.AsString);
            Assert.AreEqual(warningLogId, warningLogBson.GetElement("LogId").Value.AsString);
        }
         
        [Test]
        public void MongoDb_With_Specified_Collection_Name_Template_Without_Placeholders_Saves_Logs_To_Collections_Correctly()
        {
            //Arrange
            var database = _mongoDbTestsHelper.MongoDatabase;
            Logger logger = (Logger)LoggerBuilder.Logger("App1").ForAllLogs().MongoDb()
                                                                             .WithCollection(database, "CollectionNameWithoutPlaceholders")
                                                                             .Add()
                                                                             .BuildLogger();
            //Act
            var logId = logger.LogInfo("Context1", "Description1", new { });

            //Assert 
            var collection = database.GetCollection<BsonDocument>("CollectionNameWithoutPlaceholders");
            BsonDocument logBson = collection.Find(x => true).ToList().Single();
            Assert.AreEqual(logId, logBson.GetElement("LogId").Value.AsString);
        }

        [Test]
        public void MongoDb_With_CollectionFactory_Method_Saves_Logs_To_Collections_Correctly()
        {
            //Arrange
            var database = _mongoDbTestsHelper.MongoDatabase;
            LogModelFunc<IMongoCollection<BsonDocument>> generatorFunction = d =>
            {
                if (d.Context == "Context1")
                    return database.GetCollection<BsonDocument>("CollectionForContext1");
                else if (d.Context == "Context2")
                    return database.GetCollection<BsonDocument>("CollectionForContext2");
                else
                    return database.GetCollection<BsonDocument>("CollectionForOtherContexts");
            };

            Logger logger = (Logger)LoggerBuilder.Logger("App1").ForAllLogs().MongoDb()
                                                                             .WithCollection(generatorFunction)
                                                                             .Add()
                                                                             .BuildLogger();
            //Act
            var logId1 = logger.LogInfo("Context1", "Description1", new { });
            var logId2 = logger.LogInfo("Context2", "Description1", new { });
            var logId3 = logger.LogInfo("Context3", "Description1", new { });

            //Assert  
            BsonDocument log1Bson = database.GetCollection<BsonDocument>("CollectionForContext1").Find(x => true).ToList().Single();
            BsonDocument log2Bson = database.GetCollection<BsonDocument>("CollectionForContext2").Find(x => true).ToList().Single();
            BsonDocument log3Bson = database.GetCollection<BsonDocument>("CollectionForOtherContexts").Find(x => true).ToList().Single();

            Assert.AreEqual(logId1, log1Bson.GetElement("LogId").Value.AsString);
            Assert.AreEqual(logId2, log2Bson.GetElement("LogId").Value.AsString);
            Assert.AreEqual(logId3, log3Bson.GetElement("LogId").Value.AsString);
        }

        [Test]
        public void MongoDb_With_LogType_Collection_Dictionary_Saves_Logs_To_Collections_Correctly()
        {
            //Arrange
            var database = _mongoDbTestsHelper.MongoDatabase;
            Dictionary<LogType, IMongoCollection<BsonDocument>> dictionary = new Dictionary<LogType, IMongoCollection<BsonDocument>>()
            {
                { LogType.Success, database.GetCollection<BsonDocument>("SuccessLogs") },
                { LogType.Info, database.GetCollection<BsonDocument>("InfoLogs") },
                { LogType.Event, database.GetCollection<BsonDocument>("EventLogs") },
                { LogType.Warning, database.GetCollection<BsonDocument>("WarningLogs") },
                { LogType.Failure, database.GetCollection<BsonDocument>("FailureLogs") },
                { LogType.Error, database.GetCollection<BsonDocument>("ErrorLogs") },
                { LogType.Critical, database.GetCollection<BsonDocument>("CriticalLogs") },
            };

            Logger logger = (Logger)LoggerBuilder.Logger("App1").ForAllLogs().MongoDb()
                                                                             .WithCollection(dictionary)
                                                                             .Add()
                                                                             .BuildLogger();
            //Act
            var successLogId = logger.LogSuccess("Context", "Description");
            var infoLogId = logger.LogInfo("Context", "Description");
            var eventLogId = logger.LogEvent("Context", "Description");
            var warningLogId = logger.LogWarning("Context", "Description");
            var failureLogId = logger.LogFailure("Context", "Description");
            var errorLogId = logger.LogError("Context", "Description");
            var criticalLogId = logger.LogCritical("Context", "Description");

            //Assert  
            BsonDocument successLogBson = database.GetCollection<BsonDocument>("SuccessLogs").Find(x => true).ToList().Single();
            BsonDocument infoLogBson = database.GetCollection<BsonDocument>("InfoLogs").Find(x => true).ToList().Single();
            BsonDocument eventLogBson = database.GetCollection<BsonDocument>("EventLogs").Find(x => true).ToList().Single();
            BsonDocument warningLogBson = database.GetCollection<BsonDocument>("WarningLogs").Find(x => true).ToList().Single();
            BsonDocument failureLogBson = database.GetCollection<BsonDocument>("FailureLogs").Find(x => true).ToList().Single();
            BsonDocument errorLogBson = database.GetCollection<BsonDocument>("ErrorLogs").Find(x => true).ToList().Single();
            BsonDocument criticalLogBson = database.GetCollection<BsonDocument>("CriticalLogs").Find(x => true).ToList().Single();

            Assert.AreEqual(successLogId, successLogBson.GetElement("LogId").Value.AsString);
            Assert.AreEqual(infoLogId, infoLogBson.GetElement("LogId").Value.AsString);
            Assert.AreEqual(eventLogId, eventLogBson.GetElement("LogId").Value.AsString);
            Assert.AreEqual(warningLogId, warningLogBson.GetElement("LogId").Value.AsString);
            Assert.AreEqual(failureLogId, failureLogBson.GetElement("LogId").Value.AsString);
            Assert.AreEqual(errorLogId, errorLogBson.GetElement("LogId").Value.AsString);
            Assert.AreEqual(criticalLogId, criticalLogBson.GetElement("LogId").Value.AsString);
        }

        [Test]
        public async Task MongoDb_With_Custom_Collection_Provider_Saves_Logs_To_Collections_Correctly()
        {
            //Arrange 
            var collection = _mongoDbTestsHelper.MongoDatabase.GetCollection<BsonDocument>("collection");
            var customCollectionProviderMock = new Mock<IMongoCollectionProvider>();
            customCollectionProviderMock.Setup(x => x.GetCollectionAsync(It.IsAny<LogModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            Logger logger = (Logger)LoggerBuilder.Logger("App1").ForAllLogs().MongoDb()
                                                                             .WithCollection(customCollectionProviderMock.Object)
                                                                             .Add()
                                                                             .BuildLogger();
            //Act
            var logId = await logger.LogSuccessAsync("Context", "Description");

            //Assert  
            BsonDocument logBson = _mongoDbTestsHelper.MongoDatabase.GetCollection<BsonDocument>("collection").Find(x => true).ToList().Single();
            Assert.AreEqual(logId, logBson.GetElement("LogId").Value.AsString);
        }

        [Test]
        public async Task MongoDb_With_ConverterFunctionBsonDocumentBuilder_Saves_Logs_In_Right_Representation_To_The_Right_Collection()
        {
            //Arrange 
            var collection = _mongoDbTestsHelper.MongoDatabase.GetCollection<BsonDocument>("collection");
            var customCollectionProviderMock = new Mock<IMongoCollectionProvider>();
            customCollectionProviderMock.Setup(x => x.GetCollectionAsync(It.IsAny<LogModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            var customLogObj = new { Name = "Name1" };
            LogModelFunc<BsonDocument> bsonDocumentGeneratorFunction = d =>
            {
                return customLogObj.ToBsonDocument();
            };
            Logger logger = (Logger)LoggerBuilder.Logger("App1").ForAllLogs().MongoDb()
                                                                             .WithBsonDocument(bsonDocumentGeneratorFunction)
                                                                             .WithCollection(customCollectionProviderMock.Object)
                                                                             .Add()
                                                                             .BuildLogger();
            //Act
            var logId = await logger.LogSuccessAsync("Context", "Description");

            //Assert
            BsonDocument logBson = _mongoDbTestsHelper.MongoDatabase.GetCollection<BsonDocument>("collection").Find(x => true).ToList().Single();
            Assert.AreEqual(customLogObj.Name, logBson.GetElement("Name").Value.AsString);
        }

        [Test]
        public async Task MongoDb_With_CustomBsonDocumentBuilder_Saves_Logs_In_Right_Representation_To_The_Right_Collection()
        {
            //Arrange 
            var collection = _mongoDbTestsHelper.MongoDatabase.GetCollection<BsonDocument>("collection");
            var customCollectionProviderMock = new Mock<IMongoCollectionProvider>();
            customCollectionProviderMock.Setup(x => x.GetCollectionAsync(It.IsAny<LogModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            var customLogObj = new { Name = "Name1" };
            var customBsonDocumentBuilderMock = new Mock<IBsonDocumentBuilder>();
            customBsonDocumentBuilderMock.Setup(x => x.BuildFromAsync(It.IsAny<LogModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(customLogObj.ToBsonDocument());

            Logger logger = (Logger)LoggerBuilder.Logger("App1").ForAllLogs().MongoDb()
                                                                             .WithBsonDocument(customBsonDocumentBuilderMock.Object)
                                                                             .WithCollection(customCollectionProviderMock.Object)
                                                                             .Add()
                                                                             .BuildLogger();
            //Act
            var logId = await logger.LogSuccessAsync("Context", "Description");

            //Assert
            BsonDocument logBson = _mongoDbTestsHelper.MongoDatabase.GetCollection<BsonDocument>("collection").Find(x => true).ToList().Single();
            Assert.AreEqual(customLogObj.Name, logBson.GetElement("Name").Value.AsString);
        }

        [Test]
        public async Task LoggerBuilder_With_Two_Log_Groups_With_One_MongoDbDestination_In_Each_Saves_Logs_In_Right_Destinations()
        {
            //Arrange 
            var errorsCollection = _mongoDbTestsHelper.MongoDatabase.GetCollection<BsonDocument>("errorsCollection");
            var eventsCollection = _mongoDbTestsHelper.MongoDatabase.GetCollection<BsonDocument>("eventsCollection");
            Logger logger = (Logger)LoggerBuilder.Logger("App1").NewLogGroup("Errors")
                                                                .ForErrorLogs()
                                                                .MongoDb()
                                                                    .WithCollection(errorsCollection)
                                                                    .Add()
                                                                .BuildLogGroup()
                                                                .NewLogGroup("Events")
                                                                .ForEventLogs()
                                                                .MongoDb()
                                                                    .WithCollection(eventsCollection)
                                                                    .Add()
                                                                .BuildLogGroup()
                                                                .BuildLogger();
            //Act
            var errorLogId = await logger.LogErrorAsync("Context", "Error");
            var eventLogId = await logger.LogEventAsync("Context", "Event");

            //Assert
            BsonDocument errorLogBson = _mongoDbTestsHelper.MongoDatabase.GetCollection<BsonDocument>("errorsCollection").Find(x => true).ToList().Single();
            BsonDocument eventLogBson = _mongoDbTestsHelper.MongoDatabase.GetCollection<BsonDocument>("eventsCollection").Find(x => true).ToList().Single();
            Assert.AreEqual(errorLogId, errorLogBson.GetElement("LogId").Value.AsString);
            Assert.AreEqual(eventLogId, eventLogBson.GetElement("LogId").Value.AsString);
        }

        [Test]
        public async Task LoggerBuilder_With_One_Log_Group_Containing_Two_MongoDbDestination_All_Logs_Are_Written_To_Both_Collections_Saves_Logs_In_Right_Destinations()
        {
            //Arrange 
            var collection1 = _mongoDbTestsHelper.MongoDatabase.GetCollection<BsonDocument>("collection1");
            var collection2 = _mongoDbTestsHelper.MongoDatabase.GetCollection<BsonDocument>("collection2");
            Logger logger = (Logger)LoggerBuilder.Logger("App1").NewLogGroup("Errors")
                                                                .ForAllLogs()
                                                                .MongoDb()
                                                                    .WithCollection(collection1)
                                                                    .Add()
                                                                .MongoDb()
                                                                    .WithCollection(collection2)
                                                                    .Add()
                                                                .BuildLogGroup()
                                                                .BuildLogger();
            //Act
            var log1Id = await logger.LogErrorAsync("Context", "Error");
            var log2Id = await logger.LogEventAsync("Context", "Event");

            //Assert
            List<BsonDocument> logBsonList1 = _mongoDbTestsHelper.MongoDatabase.GetCollection<BsonDocument>("collection1").Find(x => true).ToList();
            List<BsonDocument> logBsonList2 = _mongoDbTestsHelper.MongoDatabase.GetCollection<BsonDocument>("collection2").Find(x => true).ToList();
            Assert.AreEqual(2, logBsonList1.Count);
            Assert.AreEqual(2, logBsonList2.Count);
            Assert.NotNull(logBsonList1.FirstOrDefault(d => d.GetElement("LogId").Value.AsString == log1Id));
            Assert.NotNull(logBsonList1.FirstOrDefault(d => d.GetElement("LogId").Value.AsString == log2Id));
            Assert.NotNull(logBsonList2.FirstOrDefault(d => d.GetElement("LogId").Value.AsString == log1Id));
            Assert.NotNull(logBsonList2.FirstOrDefault(d => d.GetElement("LogId").Value.AsString == log2Id));
        }
    }
}
