using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TacitusLogger.Destinations;

namespace TacitusLogger.Destinations.MongoDb.UnitTests
{
    internal class LogPack
    {
        public List<BsonDocument> BsonDocs { get; set; }
        public Mock<IMongoCollection<BsonDocument>> MongoCollectionMock { get; set; }
    }

    [TestFixture]
    public class MongoDbDestinationTests
    {
        private bool FirstListContainsAllAndOnlyElementsOfTheSecond<T>(List<T> firstList, List<T> secondList)
        {
            return firstList.ToJson() == secondList.ToJson();
        }

        #region Ctor tests

        [Test]
        public void Ctor_Taking_MongoCollectionProvider_And_BsonDocumentBuilder_When_Called_Sets_MongoCollectionProvider_And_BsonDocumentBuilder()
        {
            //Arrange
            IMongoCollectionProvider mongoCollectionProvider = new Mock<IMongoCollectionProvider>().Object;
            IBsonDocumentBuilder bsonDocumentBuilder = new Mock<IBsonDocumentBuilder>().Object;

            //Act
            MongoDbDestination mongoDbDestination = new MongoDbDestination(mongoCollectionProvider, bsonDocumentBuilder);

            //Assert
            Assert.AreEqual(mongoCollectionProvider, mongoDbDestination.CollectionProvider);
            Assert.AreEqual(bsonDocumentBuilder, mongoDbDestination.BsonDocumentBuilder);
        }

        [Test]
        public void Ctor_Taking_MongoCollectionProvider_And_BsonDocumentBuilder_When_Called_With_Null_MongoCollectionProvider_Throws_ArgumentNullException()
        {
            //Arrange 
            IBsonDocumentBuilder bsonDocumentBuilder = new Mock<IBsonDocumentBuilder>().Object;

            //Assert
            Assert.Catch<ArgumentNullException>(() =>
            {
                //Act
                MongoDbDestination mongoDbDestination = new MongoDbDestination(null as IMongoCollectionProvider, bsonDocumentBuilder);
            });
        }

        [Test]
        public void Ctor_Taking_MongoCollectionProvider_And_BsonDocumentBuilder_When_Called_With_Null_BsonDocumentBuilder_Throws_ArgumentNullException()
        {
            //Arrange 
            IMongoCollectionProvider mongoCollectionProvider = new Mock<IMongoCollectionProvider>().Object;

            //Assert
            Assert.Catch<ArgumentNullException>(() =>
            {
                //Act
                MongoDbDestination mongoDbDestination = new MongoDbDestination(mongoCollectionProvider, null as IBsonDocumentBuilder);
            });
        }

        #endregion

        #region Tests for Send method

        [Test]
        public void Send_When_Called_Calls_BsonDocumentBuilders_Build_From_Method()
        {
            //Arrange
            var bsonDocumentBuilderMock = new Mock<IBsonDocumentBuilder>();
            var mongoCollectionMock = new Mock<IMongoCollection<BsonDocument>>();
            var mongoCollectionProviderMock = new Mock<IMongoCollectionProvider>();
            LogModel logModel = new LogModel();
            BsonDocument bsonDoc = new BsonDocument();
            bsonDocumentBuilderMock.Setup(x => x.BuildFrom(logModel)).Returns(bsonDoc);
            mongoCollectionProviderMock.Setup(x => x.GetCollection(logModel)).Returns(mongoCollectionMock.Object);
            MongoDbDestination mongoDbDestination = new MongoDbDestination(mongoCollectionProviderMock.Object, bsonDocumentBuilderMock.Object);

            //Act
            mongoDbDestination.Send(new LogModel[] { logModel });

            //Assert
            bsonDocumentBuilderMock.Verify(x => x.BuildFrom(logModel), Times.Once);
        }

        [Test]
        public void Send_When_BsonDocumentBuilders_BuildFrom_Method_Returns_Null_Throws_ArgumentNullException()
        {
            //Arrange
            var mongoCollectionProviderMock = new Mock<IMongoCollectionProvider>();
            var bsonDocumentBuilderMock = new Mock<IBsonDocumentBuilder>();
            LogModel logModel = new LogModel();
            bsonDocumentBuilderMock.Setup(x => x.BuildFrom(logModel)).Returns(null as BsonDocument);
            MongoDbDestination mongoDbDestination = new MongoDbDestination(mongoCollectionProviderMock.Object, bsonDocumentBuilderMock.Object);

            //Assert
            Assert.Catch<Exception>(() =>
            {
                //Act
                mongoDbDestination.Send(new LogModel[] { logModel });
            });
            bsonDocumentBuilderMock.Verify(x => x.BuildFrom(logModel), Times.Once);
            mongoCollectionProviderMock.Verify(x => x.GetCollection(logModel), Times.Never);
        }

        [Test]
        public void Send_When_Called_Calls_MongoCollectionProviders_GetCollection_Method()
        {
            //Arrange
            var bsonDocumentBuilderMock = new Mock<IBsonDocumentBuilder>();
            var mongoCollectionMock = new Mock<IMongoCollection<BsonDocument>>();
            var mongoCollectionProviderMock = new Mock<IMongoCollectionProvider>();
            LogModel logModel = new LogModel();
            BsonDocument bsonDoc = new BsonDocument();
            bsonDocumentBuilderMock.Setup(x => x.BuildFrom(logModel)).Returns(bsonDoc);
            mongoCollectionProviderMock.Setup(x => x.GetCollection(logModel)).Returns(mongoCollectionMock.Object);
            MongoDbDestination mongoDbDestination = new MongoDbDestination(mongoCollectionProviderMock.Object, bsonDocumentBuilderMock.Object);

            //Act
            mongoDbDestination.Send(new LogModel[] { logModel });

            //Assert
            mongoCollectionProviderMock.Verify(x => x.GetCollection(logModel), Times.Once);
        }

        [Test]
        public void Send_When_MongoCollectionProviders_GetCollection_Method_Returns_Null_Throws_ArgumentNullException()
        {
            //Arrange
            var mongoCollectionProviderMock = new Mock<IMongoCollectionProvider>();
            var bsonDocumentBuilderMock = new Mock<IBsonDocumentBuilder>();
            LogModel logModel = new LogModel();
            bsonDocumentBuilderMock.Setup(x => x.BuildFrom(logModel)).Returns(new BsonDocument());
            mongoCollectionProviderMock.Setup(x => x.GetCollection(logModel)).Returns(null as IMongoCollection<BsonDocument>);
            MongoDbDestination mongoDbDestination = new MongoDbDestination(mongoCollectionProviderMock.Object, bsonDocumentBuilderMock.Object);

            //Assert
            Assert.Catch<Exception>(() =>
            {
                //Act
                mongoDbDestination.Send(new LogModel[] { logModel });
            });
            mongoCollectionProviderMock.Verify(x => x.GetCollection(logModel), Times.Once);
        }

        [Test]
        public void Send_When_Called_MongoCollections_InsertOne_Method_Is_Called()
        {
            //Arrange
            var mongoCollectionProviderMock = new Mock<IMongoCollectionProvider>();
            var bsonDocumentBuilderMock = new Mock<IBsonDocumentBuilder>();
            var mongoCollectionMock = new Mock<IMongoCollection<BsonDocument>>();
            LogModel logModel = new LogModel();
            var bsonDoc = new BsonDocument();
            bsonDocumentBuilderMock.Setup(x => x.BuildFrom(logModel)).Returns(bsonDoc);
            mongoCollectionProviderMock.Setup(x => x.GetCollection(logModel)).Returns(mongoCollectionMock.Object);
            MongoDbDestination mongoDbDestination = new MongoDbDestination(mongoCollectionProviderMock.Object, bsonDocumentBuilderMock.Object);

            //Act
            mongoDbDestination.Send(new LogModel[] { logModel });

            //Assert
            mongoCollectionMock.Verify(x => x.InsertOne(bsonDoc, null, default(CancellationToken)), Times.Once);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(15)]
        public void Send_When_Called_With_N_Different_LogData_That_Corresponds_To_N_Different_Mongo_Collections_Calls_Each_Mongo_Collection_Method_One_Time(int N)
        {
            // Arrange 
            var mongoCollectionProviderMock = new Mock<IMongoCollectionProvider>();
            var bsonDocumentBuilderMock = new Mock<IBsonDocumentBuilder>();

            var collectionDict = new Dictionary<LogModel, Mock<IMongoCollection<BsonDocument>>>();
            var bsonDocDict = new Dictionary<LogModel, BsonDocument>();
            for (int i = 0; i < N; i++)
            {
                var logModel = new LogModel() { Description = $"logText{i}" };
                collectionDict.Add(logModel, new Mock<IMongoCollection<BsonDocument>>());
                bsonDocDict.Add(logModel, logModel.ToBsonDocument());
            }

            mongoCollectionProviderMock.Setup(x => x.GetCollection(It.IsAny<LogModel>())).Returns((LogModel x) => collectionDict[x].Object);
            bsonDocumentBuilderMock.Setup(x => x.BuildFrom(It.IsAny<LogModel>())).Returns((LogModel x) => bsonDocDict[x]);

            MongoDbDestination mongoDbDestination = new MongoDbDestination(mongoCollectionProviderMock.Object, bsonDocumentBuilderMock.Object);

            // Act
            mongoDbDestination.Send(bsonDocDict.Keys.ToArray());

            // Assert  
            foreach (LogModel logModel in bsonDocDict.Keys)
            {
                collectionDict[logModel].Verify(x => x.InsertOne(bsonDocDict[logModel], null, default(CancellationToken)), Times.Once);
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(15)]
        public void Send_When_Called_With_N_Different_LogData_That_Corresponds_To_The_Same_Mongo_Collection_Calls_Mongo_Collection_Method_Once_And_Passes_BsonDocuments(int N)
        {
            // Arrange 
            var mongoCollectionProviderMock = new Mock<IMongoCollectionProvider>();
            var mongoCollectionMock = new Mock<IMongoCollection<BsonDocument>>();
            var bsonDocumentBuilderMock = new Mock<IBsonDocumentBuilder>();

            var bsonDocDict = new Dictionary<LogModel, BsonDocument>();
            for (int i = 0; i < N; i++)
            {
                var logModel = new LogModel() { Description = $"logText{i}" };
                bsonDocDict.Add(logModel, logModel.ToBsonDocument());
            }

            mongoCollectionProviderMock.Setup(x => x.GetCollection(It.IsAny<LogModel>())).Returns(mongoCollectionMock.Object);
            bsonDocumentBuilderMock.Setup(x => x.BuildFrom(It.IsAny<LogModel>())).Returns((LogModel x) => bsonDocDict[x]);
            MongoDbDestination mongoDbDestination = new MongoDbDestination(mongoCollectionProviderMock.Object, bsonDocumentBuilderMock.Object);

            // Act
            mongoDbDestination.Send(bsonDocDict.Keys.ToArray());

            // Assert
            if (N == 1)
                mongoCollectionMock.Verify(x => x.InsertOne(It.Is<BsonDocument>(d => d == bsonDocDict.Values.First()), null, default(CancellationToken)), Times.Once);
            else
                mongoCollectionMock.Verify(x => x.InsertMany(It.Is<IEnumerable<BsonDocument>>(d => FirstListContainsAllAndOnlyElementsOfTheSecond<BsonDocument>(d.ToList(), bsonDocDict.Values.ToList())), null, default(CancellationToken)), Times.Once);
        }

        [TestCase(5)]
        [TestCase(7)]
        [TestCase(9)]
        [TestCase(11)]
        [TestCase(15)]
        public void Send_When_Called_With_N_Logs_Resulting_In_M_Mongo_Collections_Destinations_Calls_Each_Mongo_Collection_Method_M_Times_And_Passes_BsonDocuments(int N)
        {
            // Arrange 
            var mongoCollectionProviderMock = new Mock<IMongoCollectionProvider>();
            var bsonDocumentBuilderMock = new Mock<IBsonDocumentBuilder>();
            LogModel[] logs = new LogModel[N];
            BsonDocument[] bsonDocuments = new BsonDocument[N];
            List<BsonDocument>[] bsonDocGroups = new List<BsonDocument>[5];
            LogPack[] logPacks = new LogPack[5];
            for (int j = 0; j < 5; j++)
            {
                logPacks[j] = new LogPack();
                logPacks[j].MongoCollectionMock = new Mock<IMongoCollection<BsonDocument>>();
                logPacks[j].BsonDocs = new List<BsonDocument>();
            }
            for (int i = 0; i < N; i++)
            {
                logs[i] = new LogModel() { Description = $"logText{i}" };
                bsonDocuments[i] = logs[i].ToBsonDocument();
                logPacks[i % 5].BsonDocs.Add(bsonDocuments[i]);
            }
            mongoCollectionProviderMock.Setup(x => x.GetCollection(It.IsAny<LogModel>())).Returns((LogModel x) => logPacks[logs.ToList().IndexOf(x) % 5].MongoCollectionMock.Object);
            bsonDocumentBuilderMock.Setup(x => x.BuildFrom(It.IsAny<LogModel>())).Returns((LogModel x) => bsonDocuments[logs.ToList().IndexOf(x)]);
            MongoDbDestination mongoDbDestination = new MongoDbDestination(mongoCollectionProviderMock.Object, bsonDocumentBuilderMock.Object);

            // Act
            mongoDbDestination.Send(logs);

            // Assert
            for (int j = 0; j < 5; j++)
            {
                if (logPacks[j].BsonDocs.Count == 1)
                    logPacks[j].MongoCollectionMock.Verify(x => x.InsertOne(logPacks[j].BsonDocs[0], null, default(CancellationToken)));
                else
                    logPacks[j].MongoCollectionMock.Verify(x => x.InsertMany(logPacks[j].BsonDocs, null, default(CancellationToken)));
            }
        }

        #endregion

        #region Tests for SendAsync method

        [Test]
        public async Task SendAsync_When_Called_Calls_BsonDocumentBuilders_BuildFrom_Method()
        {
            //Arrange
            var bsonDocumentBuilderMock = new Mock<IBsonDocumentBuilder>();
            var mongoCollectionMock = new Mock<IMongoCollection<BsonDocument>>();
            var mongoCollectionProviderMock = new Mock<IMongoCollectionProvider>();
            LogModel logModel = new LogModel();
            BsonDocument bsonDoc = new BsonDocument();
            bsonDocumentBuilderMock.Setup(x => x.BuildFromAsync(logModel, It.IsAny<CancellationToken>())).ReturnsAsync(bsonDoc);
            mongoCollectionProviderMock.Setup(x => x.GetCollectionAsync(logModel, It.IsAny<CancellationToken>())).ReturnsAsync(mongoCollectionMock.Object);
            MongoDbDestination mongoDbDestination = new MongoDbDestination(mongoCollectionProviderMock.Object, bsonDocumentBuilderMock.Object);
            CancellationToken cancellationToken = new CancellationToken();

            //Act
            await mongoDbDestination.SendAsync(new LogModel[] { logModel }, cancellationToken);

            //Assert
            bsonDocumentBuilderMock.Verify(x => x.BuildFromAsync(logModel, cancellationToken), Times.Once);
        }

        [Test]
        public void SendAsync_When_BsonDocumentBuilders_BuildFrom_Method_Returns_Null_Throws_ArgumentNullException()
        {
            //Arrange
            var mongoCollectionProviderMock = new Mock<IMongoCollectionProvider>();
            var bsonDocumentBuilderMock = new Mock<IBsonDocumentBuilder>();
            LogModel logModel = new LogModel();
            bsonDocumentBuilderMock.Setup(x => x.BuildFromAsync(logModel, It.IsAny<CancellationToken>())).ReturnsAsync(null as BsonDocument);
            MongoDbDestination mongoDbDestination = new MongoDbDestination(mongoCollectionProviderMock.Object, bsonDocumentBuilderMock.Object);
            CancellationToken cancellationToken = new CancellationToken();

            //Assert
            Assert.CatchAsync<Exception>(async () =>
            {
                //Act
                await mongoDbDestination.SendAsync(new LogModel[] { logModel });
            });
            bsonDocumentBuilderMock.Verify(x => x.BuildFromAsync(logModel, cancellationToken), Times.Once);
            mongoCollectionProviderMock.Verify(x => x.GetCollectionAsync(logModel, It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task SendAsync_When_Called_Calls_GetCollection_Method_Of_MongoCollectionProvider()
        {
            //Arrange
            var bsonDocumentBuilderMock = new Mock<IBsonDocumentBuilder>();
            var mongoCollectionMock = new Mock<IMongoCollection<BsonDocument>>();
            var mongoCollectionProviderMock = new Mock<IMongoCollectionProvider>();
            LogModel logModel = new LogModel();
            BsonDocument bsonDoc = new BsonDocument();
            bsonDocumentBuilderMock.Setup(x => x.BuildFromAsync(logModel, It.IsAny<CancellationToken>())).ReturnsAsync(bsonDoc);
            mongoCollectionProviderMock.Setup(x => x.GetCollectionAsync(logModel, It.IsAny<CancellationToken>())).ReturnsAsync(mongoCollectionMock.Object);
            MongoDbDestination mongoDbDestination = new MongoDbDestination(mongoCollectionProviderMock.Object, bsonDocumentBuilderMock.Object);
            CancellationToken cancellationToken = new CancellationToken();

            //Act
            await mongoDbDestination.SendAsync(new LogModel[] { logModel }, cancellationToken);

            //Assert
            mongoCollectionProviderMock.Verify(x => x.GetCollectionAsync(logModel, cancellationToken), Times.Once);
        }

        [Test]
        public void SendAsync_When_MongoCollectionProviders_GetCollection_Method_Returns_Null_Throws_ArgumentNullException()
        {
            //Arrange
            var mongoCollectionProviderMock = new Mock<IMongoCollectionProvider>();
            var bsonDocumentBuilderMock = new Mock<IBsonDocumentBuilder>();
            LogModel logModel = new LogModel();
            bsonDocumentBuilderMock.Setup(x => x.BuildFromAsync(logModel, It.IsAny<CancellationToken>())).ReturnsAsync(new BsonDocument());
            mongoCollectionProviderMock.Setup(x => x.GetCollectionAsync(logModel, It.IsAny<CancellationToken>())).ReturnsAsync(null as IMongoCollection<BsonDocument>);
            MongoDbDestination mongoDbDestination = new MongoDbDestination(mongoCollectionProviderMock.Object, bsonDocumentBuilderMock.Object);
            CancellationToken cancellationToken = new CancellationToken();

            //Assert
            Assert.CatchAsync<Exception>(async () =>
            {
                //Act
                await mongoDbDestination.SendAsync(new LogModel[] { logModel }, cancellationToken);
            });
            mongoCollectionProviderMock.Verify(x => x.GetCollectionAsync(logModel, cancellationToken), Times.Once);
        }

        [Test]
        public async Task SendAsync_When_Called_MongoCollections_InsertOne_Method_Is_Called()
        {
            //Arrange
            var mongoCollectionProviderMock = new Mock<IMongoCollectionProvider>();
            var bsonDocumentBuilderMock = new Mock<IBsonDocumentBuilder>();
            var mongoCollectionMock = new Mock<IMongoCollection<BsonDocument>>();
            LogModel logModel = new LogModel();
            var bsonDoc = new BsonDocument();
            bsonDocumentBuilderMock.Setup(x => x.BuildFromAsync(logModel, It.IsAny<CancellationToken>())).ReturnsAsync(bsonDoc);
            mongoCollectionProviderMock.Setup(x => x.GetCollectionAsync(logModel, It.IsAny<CancellationToken>())).ReturnsAsync(mongoCollectionMock.Object);
            MongoDbDestination mongoDbDestination = new MongoDbDestination(mongoCollectionProviderMock.Object, bsonDocumentBuilderMock.Object);
            CancellationToken cancellationToken = new CancellationToken();

            //Act
            await mongoDbDestination.SendAsync(new LogModel[] { logModel }, cancellationToken);

            //Assert
            mongoCollectionMock.Verify(x => x.InsertOneAsync(bsonDoc, null, cancellationToken), Times.Once);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(15)]
        public async Task SendAsync_When_Called_With_N_Different_LogData_That_Corresponds_To_N_Different_Mongo_Collections_Calls_Each_Mongo_Collection_Method_One_Time(int N)
        {
            // Arrange 
            var mongoCollectionProviderMock = new Mock<IMongoCollectionProvider>();
            var bsonDocumentBuilderMock = new Mock<IBsonDocumentBuilder>();

            var collectionDict = new Dictionary<LogModel, Mock<IMongoCollection<BsonDocument>>>();
            var bsonDocDict = new Dictionary<LogModel, BsonDocument>();
            for (int i = 0; i < N; i++)
            {
                var logModel = new LogModel() { Description = $"logText{i}" };
                collectionDict.Add(logModel, new Mock<IMongoCollection<BsonDocument>>());
                bsonDocDict.Add(logModel, logModel.ToBsonDocument());
            }

            mongoCollectionProviderMock.Setup(x => x.GetCollectionAsync(It.IsAny<LogModel>(), It.IsAny<CancellationToken>())).ReturnsAsync((LogModel x, CancellationToken t) => collectionDict[x].Object);
            bsonDocumentBuilderMock.Setup(x => x.BuildFromAsync(It.IsAny<LogModel>(), It.IsAny<CancellationToken>())).ReturnsAsync((LogModel x, CancellationToken t) => bsonDocDict[x]);
            MongoDbDestination mongoDbDestination = new MongoDbDestination(mongoCollectionProviderMock.Object, bsonDocumentBuilderMock.Object);
            CancellationToken cancellationToken = new CancellationToken();

            // Act
            await mongoDbDestination.SendAsync(bsonDocDict.Keys.ToArray(), cancellationToken);

            // Assert  
            foreach (LogModel logModel in bsonDocDict.Keys)
            {
                collectionDict[logModel].Verify(x => x.InsertOneAsync(bsonDocDict[logModel], null, cancellationToken), Times.Once);
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(15)]
        public async Task SendAsync_When_Called_With_N_Different_LogData_That_Corresponds_To_The_Same_Mongo_Collection_Calls_Mongo_Collection_Method_Once_And_Passes_BsonDocuments(int N)
        {
            // Arrange 
            var mongoCollectionProviderMock = new Mock<IMongoCollectionProvider>();
            var mongoCollectionMock = new Mock<IMongoCollection<BsonDocument>>();
            var bsonDocumentBuilderMock = new Mock<IBsonDocumentBuilder>();

            var bsonDocDict = new Dictionary<LogModel, BsonDocument>();
            for (int i = 0; i < N; i++)
            {
                var logModel = new LogModel() { Description = $"logText{i}" };
                bsonDocDict.Add(logModel, logModel.ToBsonDocument());
            }

            mongoCollectionProviderMock.Setup(x => x.GetCollectionAsync(It.IsAny<LogModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(mongoCollectionMock.Object);
            bsonDocumentBuilderMock.Setup(x => x.BuildFromAsync(It.IsAny<LogModel>(), It.IsAny<CancellationToken>())).ReturnsAsync((LogModel x, CancellationToken t) => bsonDocDict[x]);
            MongoDbDestination mongoDbDestination = new MongoDbDestination(mongoCollectionProviderMock.Object, bsonDocumentBuilderMock.Object);
            CancellationToken cancellationToken = new CancellationToken();

            // Act
            await mongoDbDestination.SendAsync(bsonDocDict.Keys.ToArray(), cancellationToken);

            // Assert
            if (N == 1)
                mongoCollectionMock.Verify(x => x.InsertOneAsync(It.Is<BsonDocument>(d => d == bsonDocDict.Values.First()), null, cancellationToken), Times.Once);
            else
                mongoCollectionMock.Verify(x => x.InsertManyAsync(It.Is<IEnumerable<BsonDocument>>(d => FirstListContainsAllAndOnlyElementsOfTheSecond<BsonDocument>(d.ToList(), bsonDocDict.Values.ToList())), null, cancellationToken), Times.Once);
        }

        [TestCase(5)]
        [TestCase(7)]
        [TestCase(9)]
        [TestCase(11)]
        [TestCase(15)]
        public async Task SendAsync_When_Called_With_N_Logs_Resulting_In_M_Mongo_Collections_Destinations_Calls_Each_Mongo_Collection_Method_M_Times_And_Passes_BsonDocuments(int N)
        {
            // Arrange 
            var mongoCollectionProviderMock = new Mock<IMongoCollectionProvider>();
            var bsonDocumentBuilderMock = new Mock<IBsonDocumentBuilder>();
            LogModel[] logs = new LogModel[N];
            BsonDocument[] bsonDocuments = new BsonDocument[N];
            List<BsonDocument>[] bsonDocGroups = new List<BsonDocument>[5];
            LogPack[] logPacks = new LogPack[5];
            for (int j = 0; j < 5; j++)
            {
                logPacks[j] = new LogPack();
                logPacks[j].MongoCollectionMock = new Mock<IMongoCollection<BsonDocument>>();
                logPacks[j].BsonDocs = new List<BsonDocument>();
            }
            for (int i = 0; i < N; i++)
            {
                logs[i] = new LogModel() { Description = $"logText{i}" };
                bsonDocuments[i] = logs[i].ToBsonDocument();
                logPacks[i % 5].BsonDocs.Add(bsonDocuments[i]);
            }
            mongoCollectionProviderMock.Setup(x => x.GetCollectionAsync(It.IsAny<LogModel>(), It.IsAny<CancellationToken>())).ReturnsAsync((LogModel x, CancellationToken t) => logPacks[logs.ToList().IndexOf(x) % 5].MongoCollectionMock.Object);
            bsonDocumentBuilderMock.Setup(x => x.BuildFromAsync(It.IsAny<LogModel>(), It.IsAny<CancellationToken>())).ReturnsAsync((LogModel x, CancellationToken t) => bsonDocuments[logs.ToList().IndexOf(x)]);
            MongoDbDestination mongoDbDestination = new MongoDbDestination(mongoCollectionProviderMock.Object, bsonDocumentBuilderMock.Object);
            CancellationToken cancellationToken = new CancellationToken();

            // Act
            await mongoDbDestination.SendAsync(logs, cancellationToken);

            // Assert
            for (int j = 0; j < 5; j++)
            {
                if (logPacks[j].BsonDocs.Count == 1)
                    logPacks[j].MongoCollectionMock.Verify(x => x.InsertOneAsync(logPacks[j].BsonDocs[0], null, cancellationToken));
                else
                    logPacks[j].MongoCollectionMock.Verify(x => x.InsertManyAsync(logPacks[j].BsonDocs, null, cancellationToken));
            }
        }

        #endregion
    }
}
