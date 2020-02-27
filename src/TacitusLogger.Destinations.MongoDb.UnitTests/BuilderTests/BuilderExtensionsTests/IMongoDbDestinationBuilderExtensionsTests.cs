using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TacitusLogger.Destinations.MongoDb.UnitTests.BuilderTests.BuilderExtensionsTests
{
    [TestFixture]
    public class IMongoDbDestinationBuilderExtensionsTests
    {
        [Test]
        public void WithCollection_Taking_MongoCollection_WhenCalled_Calls_IMongoDbDestinationBuilders_WithCollection_Method()
        {
            //Arrange
            var mongoDbDestinationBuilderMock = new Mock<IMongoDbDestinationBuilder>();
            var mongoCollection = new Mock<IMongoCollection<BsonDocument>>();

            //Act
            IMongoDbDestinationBuilderExtensions.WithCollection(mongoDbDestinationBuilderMock.Object, mongoCollection.Object);

            //Assert
            mongoDbDestinationBuilderMock.Verify(x => x.WithCollection(It.Is<DirectCollectionProvider>(p => p.Collection == mongoCollection.Object)), Times.Once);
        }
        [Test]
        public void WithCollection_Taking_MongoCollectionGeneratorFunction_When_Called_Calls_IMongoDbDestinationBuilders_WithCollection_Method()
        {
            //Arrange
            var mongoDbDestinationBuilderMock = new Mock<IMongoDbDestinationBuilder>();
            LogModelFunc<IMongoCollection<BsonDocument>> generatorFunction = x => null;

            //Act
            IMongoDbDestinationBuilderExtensions.WithCollection(mongoDbDestinationBuilderMock.Object, generatorFunction);

            //Assert
            mongoDbDestinationBuilderMock.Verify(x => x.WithCollection(It.Is<GeneratorFunctionCollectionProvider>(p => p.GeneratorFunction == generatorFunction)), Times.Once);
        }
        [Test]
        public void WithCollection_With_MongoDatabase_And_Name_Template_When_Called_Calls_IMongoDbDestinationBuilders_WithCollection_Method()
        {
            //Arrange
            var mongoDbDestinationBuilderMock = new Mock<IMongoDbDestinationBuilder>();
            var mongoDatabaseMock = new Mock<IMongoDatabase>();
            var nameTemplate = "nameTemplate";

            //Act
            IMongoDbDestinationBuilderExtensions.WithCollection(mongoDbDestinationBuilderMock.Object, mongoDatabaseMock.Object, nameTemplate);

            //Assert
            mongoDbDestinationBuilderMock.Verify(x => x.WithCollection(It.Is<NameTemplateCollectionProvider>(p => p.Database == mongoDatabaseMock.Object && p.CollectionNameTemplate == nameTemplate)), Times.Once);
        }
        [Test]
        public void WithCollection_Taking_Dictionary_When_Called_Calls_IMongoDbDestinationBuilders_WithCollection_Method()
        {
            //Arrange  
            var mongoDbDestinationBuilderMock = new Mock<IMongoDbDestinationBuilder>();
            Dictionary<LogType, IMongoCollection<BsonDocument>> dictionary = new Dictionary<LogType, IMongoCollection<BsonDocument>>()
            {
                {LogType.Success, new Mock<IMongoCollection<BsonDocument>>().Object  },
                {LogType.Info, new Mock<IMongoCollection<BsonDocument>>().Object  },
                {LogType.Event, new Mock<IMongoCollection<BsonDocument>>().Object  },
                {LogType.Warning, new Mock<IMongoCollection<BsonDocument>>().Object  },
                {LogType.Failure, new Mock<IMongoCollection<BsonDocument>>().Object  },
                {LogType.Error, new Mock<IMongoCollection<BsonDocument>>().Object  },
                {LogType.Critical, new Mock<IMongoCollection<BsonDocument>>().Object  },
            };

            //Act
            IMongoDbDestinationBuilderExtensions.WithCollection(mongoDbDestinationBuilderMock.Object, dictionary);

            //Assert
            mongoDbDestinationBuilderMock.Verify(x => x.WithCollection(It.Is<LogTypeBasedCollectionProvider>(p => p.Dictionary == dictionary)), Times.Once);
        }
        [Test]
        public void WithLogModelBsonDocument_When_Called_Calls_IMongoDbDestinationBuilders_WithCollection_Method()
        {
            //Arrange  
            var mongoDbDestinationBuilderMock = new Mock<IMongoDbDestinationBuilder>();

            //Act
            IMongoDbDestinationBuilderExtensions.WithLogModelBsonDocument(mongoDbDestinationBuilderMock.Object);

            //Assert
            mongoDbDestinationBuilderMock.Verify(x => x.WithBsonDocument(It.IsNotNull<DirectBsonDocumentBuilder>()), Times.Once);
        }
        [Test]
        public void WithBsonDocument_Taking_BsonDocument_GeneratorFunction_When_Called_Calls_IMongoDbDestinationBuilders_WithCollection_Method()
        {
            //Arrange  
            var mongoDbDestinationBuilderMock = new Mock<IMongoDbDestinationBuilder>();
            LogModelFunc<BsonDocument> generatorFunction = d => null;

            //Act
            IMongoDbDestinationBuilderExtensions.WithBsonDocument(mongoDbDestinationBuilderMock.Object, generatorFunction);

            //Assert
            mongoDbDestinationBuilderMock.Verify(x => x.WithBsonDocument(It.Is<ConverterFunctionBsonDocumentBuilder>(m => m.Converter == generatorFunction)), Times.Once);
        }
    }
}
