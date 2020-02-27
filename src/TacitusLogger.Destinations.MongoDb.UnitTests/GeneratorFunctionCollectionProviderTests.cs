using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TacitusLogger.Destinations.MongoDb.UnitTests
{
    [TestFixture]
    public class GeneratorFunctionCollectionProviderTests
    {
        #region Ctor tests

        [Test]
        public void Ctor_Taking_GeneratorFunction_When_Called_Sets_The_GeneratorFunction()
        {
            //Arrange
            LogModelFunc<IMongoCollection<BsonDocument>> generatorFunction = x => null;

            //Act
            GeneratorFunctionCollectionProvider generatorFunctionCollectionProvider = new GeneratorFunctionCollectionProvider(generatorFunction);

            //Assert
            Assert.AreEqual(generatorFunction, generatorFunctionCollectionProvider.GeneratorFunction);
        }

        [Test]
        public void Ctor_Taking_GeneratorFunction_When_Called_With_Null_GeneratorFunction_Throws_ArgumentNullException()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                //Act
                GeneratorFunctionCollectionProvider generatorFunctionCollectionProvider = new GeneratorFunctionCollectionProvider(null);
            });
        }

        #endregion

        #region Tests for GetCollection and GetCollectionAsync methods

        [Test]
        public async Task GetCollection_When_Called_Returns_Collection_Using_GeneratorFunction()
        {
            //Arrange
            var mongoCollection = new Mock<IMongoCollection<BsonDocument>>().Object;
            LogModelFunc<IMongoCollection<BsonDocument>> generatorFunction = x => mongoCollection;
            GeneratorFunctionCollectionProvider generatorFunctionCollectionProvider = new GeneratorFunctionCollectionProvider(generatorFunction);

            //Act
            var returnedCollection = generatorFunctionCollectionProvider.GetCollection(It.IsAny<LogModel>());
            var returnedCollectionAsync = await generatorFunctionCollectionProvider.GetCollectionAsync(It.IsAny<LogModel>());

            //Assert
            Assert.AreEqual(mongoCollection, returnedCollection);
            Assert.AreEqual(mongoCollection, returnedCollectionAsync);
        }

        [Test]
        public void GetCollection_When_Called_Returns_Collection_Depending_On_Condition()
        {
            //Arrange
            var mongoCollection1 = new Mock<IMongoCollection<BsonDocument>>().Object;
            var mongoCollection2 = new Mock<IMongoCollection<BsonDocument>>().Object;
            LogModelFunc<IMongoCollection<BsonDocument>> generatorFunction = (x) =>
            {
                if (x.Context == "context1")
                    return mongoCollection1;
                else return mongoCollection2;
            };
            GeneratorFunctionCollectionProvider generatorFunctionCollectionProvider = new GeneratorFunctionCollectionProvider(generatorFunction);

            //Act
            var returnedCollection1 = generatorFunctionCollectionProvider.GetCollection(new LogModel() { Context = "context1" });
            var returnedCollection2 = generatorFunctionCollectionProvider.GetCollection(new LogModel() { Context = "context2" });

            //Assert
            Assert.AreEqual(mongoCollection1, returnedCollection1);
            Assert.AreEqual(mongoCollection2, returnedCollection2);
        }

        [Test]
        public async Task GetCollectionAsync_When_Called_Returns_Collection_Depending_On_Condition()
        {
            //Arrange
            var mongoCollection1 = new Mock<IMongoCollection<BsonDocument>>().Object;
            var mongoCollection2 = new Mock<IMongoCollection<BsonDocument>>().Object;
            LogModelFunc<IMongoCollection<BsonDocument>> generatorFunction = (x) =>
            {
                if (x.Context == "context1")
                    return mongoCollection1;
                else return mongoCollection2;
            };
            GeneratorFunctionCollectionProvider generatorFunctionCollectionProvider = new GeneratorFunctionCollectionProvider(generatorFunction);

            //Act
            var returnedCollection1 =await generatorFunctionCollectionProvider.GetCollectionAsync(new LogModel() { Context = "context1" });
            var returnedCollection2 = await generatorFunctionCollectionProvider.GetCollectionAsync(new LogModel() { Context = "context2" });

            //Assert
            Assert.AreEqual(mongoCollection1, returnedCollection1);
            Assert.AreEqual(mongoCollection2, returnedCollection2);
        }

        #endregion
    }
}
