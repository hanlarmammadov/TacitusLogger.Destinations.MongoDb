using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using TacitusLogger.Builders;
using System.Linq;
using TacitusLogger.Destinations;

namespace TacitusLogger.Destinations.MongoDb.UnitTests.BuilderTests
{
    [TestFixture]
    public class MongoDbDestinationBuilderTests
    {
        #region Ctor tests

        [Test]
        public void Ctor_When_Called_Sets_The_Provided_LogGroupDestinationsBuilder()
        {
            //Arrange
            var logGroupDestinationsBuilder = new Mock<ILogGroupDestinationsBuilder>().Object;

            //Act
            MongoDbDestinationBuilder mongoDbDestinationBuilder = new MongoDbDestinationBuilder(logGroupDestinationsBuilder);

            //Assert
            Assert.AreEqual(logGroupDestinationsBuilder, mongoDbDestinationBuilder.LogGroupDestinationsBuilder);
        }

        [Test]
        public void Ctor_When_Called_CollectionProvider_And_BsonDocumentBuilder_Both_Are_Null()
        {
            //Arrange
            var logGroupDestinationsBuilder = new Mock<ILogGroupDestinationsBuilder>().Object;

            //Act
            MongoDbDestinationBuilder mongoDbDestinationBuilder = new MongoDbDestinationBuilder(logGroupDestinationsBuilder);

            //Assert
            Assert.IsNull(mongoDbDestinationBuilder.CollectionProvider);
            Assert.IsNull(mongoDbDestinationBuilder.BsonDocumentBuilder);
        }

        #endregion

        #region Tests for WithCollection method

        [Test]
        public void WithCollection_When_Called_Sets_CollectionProvider()
        {
            //Arrange
            var logGroupDestinationsBuilder = new Mock<ILogGroupDestinationsBuilder>().Object;
            var collectionProvider = new Mock<IMongoCollectionProvider>().Object;
            MongoDbDestinationBuilder mongoDbDestinationBuilder = new MongoDbDestinationBuilder(logGroupDestinationsBuilder);

            //Act
            mongoDbDestinationBuilder.WithCollection(collectionProvider);

            //Assert
            Assert.AreEqual(collectionProvider, mongoDbDestinationBuilder.CollectionProvider);
        }

        [Test]
        public void WithCollection_When_Called_Given_That_Already_Set_Throws_InvalidOperationException()
        {
            // Arrange
            var logGroupDestinationsBuilder = new Mock<ILogGroupDestinationsBuilder>().Object;
            MongoDbDestinationBuilder mongoDbDestinationBuilder = new MongoDbDestinationBuilder(logGroupDestinationsBuilder);
            // Was set the first time.
            mongoDbDestinationBuilder.WithCollection(new Mock<IMongoCollectionProvider>().Object);

            // Assert
            Assert.Catch<InvalidOperationException>(() =>
            {
                // Act
                mongoDbDestinationBuilder.WithCollection(new Mock<IMongoCollectionProvider>().Object);
            });
        }

        [Test]
        public void WithCollection_When_Called_With_Null_CollectionProvider_Throws_ArgumentNullException()
        {
            //Arrange
            var logGroupDestinationsBuilder = new Mock<ILogGroupDestinationsBuilder>().Object;
            MongoDbDestinationBuilder mongoDbDestinationBuilder = new MongoDbDestinationBuilder(logGroupDestinationsBuilder);

            //Assert
            Assert.Catch<ArgumentNullException>(() =>
            {
                //Act
                mongoDbDestinationBuilder.WithCollection(null);
            });
        }

        #endregion

        #region Tests for WithBsonDocument method

        [Test]
        public void WithBsonDocument_When_Called_Sets_BsonDocumentBuilder()
        {
            //Arrange
            var logGroupDestinationsBuilder = new Mock<ILogGroupDestinationsBuilder>().Object;
            var bsonDocumentBuilder = new Mock<IBsonDocumentBuilder>().Object;
            MongoDbDestinationBuilder mongoDbDestinationBuilder = new MongoDbDestinationBuilder(logGroupDestinationsBuilder);

            //Act
            mongoDbDestinationBuilder.WithBsonDocument(bsonDocumentBuilder);

            //Assert
            Assert.AreEqual(bsonDocumentBuilder, mongoDbDestinationBuilder.BsonDocumentBuilder);
        }

        [Test]
        public void WithBsonDocument_When_Called_Given_That_Already_Set_Throws_InvalidOperationException()
        {
            // Arrange
            var logGroupDestinationsBuilder = new Mock<ILogGroupDestinationsBuilder>().Object;
            MongoDbDestinationBuilder mongoDbDestinationBuilder = new MongoDbDestinationBuilder(logGroupDestinationsBuilder);
            // Was set the first time.
            mongoDbDestinationBuilder.WithBsonDocument(new Mock<IBsonDocumentBuilder>().Object);

            // Assert
            Assert.Catch<InvalidOperationException>(() =>
            {
                // Act
                mongoDbDestinationBuilder.WithBsonDocument(new Mock<IBsonDocumentBuilder>().Object);
            });
        }

        [Test]
        public void WithBsonDocument_When_Called_With_Null_BsonDocumentBuilder_Throws_ArgumentNullException()
        {
            //Arrange
            var logGroupDestinationsBuilder = new Mock<ILogGroupDestinationsBuilder>().Object;
            MongoDbDestinationBuilder mongoDbDestinationBuilder = new MongoDbDestinationBuilder(logGroupDestinationsBuilder);

            //Assert
            Assert.Catch<ArgumentNullException>(() =>
            {
                //Act
                mongoDbDestinationBuilder.WithBsonDocument(null);
            });
        }

        #endregion

        #region Tests for Add method

        [Test]
        public void Add_When_Called_Given_That_CollectionProvider_Was_Not_Specified_Throws_Exception()
        {
            //Arrange
            var logGroupDestinationsBuilder = new Mock<ILogGroupDestinationsBuilder>().Object;
            var bsonDocumentBuilder = new Mock<IBsonDocumentBuilder>().Object;
            MongoDbDestinationBuilder mongoDbDestinationBuilder = new MongoDbDestinationBuilder(logGroupDestinationsBuilder);
            mongoDbDestinationBuilder.WithBsonDocument(bsonDocumentBuilder);

            Assert.Catch<Exception>(() =>
            {
                //Act
                mongoDbDestinationBuilder.Add();
            });
        }

        [Test]
        public void Add_When_Called_Creates_MongoDbDestination_And_Passes_To_LogGroupDestinationBuilders_AddDestination_Method()
        {
            //Arrange
            var logGroupDestinationsBuilderMock = new Mock<ILogGroupDestinationsBuilder>();
            var collectionProvider = new Mock<IMongoCollectionProvider>().Object;
            var bsonDocumentBuilder = new Mock<IBsonDocumentBuilder>().Object;
            MongoDbDestinationBuilder mongoDbDestinationBuilder = new MongoDbDestinationBuilder(logGroupDestinationsBuilderMock.Object);
            mongoDbDestinationBuilder.WithCollection(collectionProvider);
            mongoDbDestinationBuilder.WithBsonDocument(bsonDocumentBuilder);

            //Act
            mongoDbDestinationBuilder.Add();

            //Assert
            logGroupDestinationsBuilderMock.Verify(x => x.CustomDestination(It.Is<MongoDbDestination>(d => d.BsonDocumentBuilder == bsonDocumentBuilder && d.CollectionProvider == collectionProvider)), Times.Once);
        }

        [Test]
        public void Add_When_Called_Given_That_BsonDocumentBuilder_Was_Not_Specified_Sets_DirectBsonDocumentBuilder_As_Default()
        {
            //Arrange
            var logGroupDestinationsBuilderMock = new Mock<ILogGroupDestinationsBuilder>();
            var collectionProvider = new Mock<IMongoCollectionProvider>().Object;
            MongoDbDestinationBuilder mongoDbDestinationBuilder = new MongoDbDestinationBuilder(logGroupDestinationsBuilderMock.Object);
            mongoDbDestinationBuilder.WithCollection(collectionProvider);

            //Act
            mongoDbDestinationBuilder.Add();

            //Assert
            logGroupDestinationsBuilderMock.Verify(x => x.CustomDestination(It.Is<MongoDbDestination>(d => d.BsonDocumentBuilder is DirectBsonDocumentBuilder)), Times.Once);
        }

        #endregion

    }
}
