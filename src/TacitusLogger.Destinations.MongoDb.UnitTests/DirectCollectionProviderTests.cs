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
    public class DirectCollectionProviderTests
    {
        #region Ctor tests

        [Test]
        public void Ctor_Taking_MongoCollection_When_Called_Sets_The_Provided_Collection()
        {
            //Arrange
            var mongoCollection = new Mock<IMongoCollection<BsonDocument>>().Object;

            //Act
            DirectCollectionProvider singleCollectionProvider = new DirectCollectionProvider(mongoCollection);

            //Assert
            Assert.AreEqual(mongoCollection, singleCollectionProvider.Collection);
        }

        [Test]
        public void Ctor_Taking_MongoCollection_When_Called_With_Null_Collection_Throws_ArgumentNullException()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                //Act
                DirectCollectionProvider singleCollectionProvider = new DirectCollectionProvider(null);
            });
        }

        #endregion

        #region Tests for GetCollection and GetCollectionAsync methods

        [Test]
        public async Task GetCollection_When_Called_Returns_Collection_That_Was_Provided_During_Initialization()
        {
            //Arrange
            var mongoCollection = new Mock<IMongoCollection<BsonDocument>>().Object;
            DirectCollectionProvider singleCollectionProvider = new DirectCollectionProvider(mongoCollection);

            //Act
            var returnedCollection = singleCollectionProvider.GetCollection(It.IsNotNull<LogModel>());
            var returnedCollectionAsync = await singleCollectionProvider.GetCollectionAsync(It.IsNotNull<LogModel>());

            //Assert
            Assert.AreEqual(mongoCollection, returnedCollection);
            Assert.AreEqual(mongoCollection, returnedCollectionAsync);
        }

        [Test]
        public async Task GetCollection_When_Called_With_Null_LogData_Returns_Collection_That_Was_Provided_During_Initialization()
        {
            //Arrange
            var mongoCollection = new Mock<IMongoCollection<BsonDocument>>().Object;
            DirectCollectionProvider singleCollectionProvider = new DirectCollectionProvider(mongoCollection);

            //Act
            var returnedCollection = singleCollectionProvider.GetCollection(null);
            var returnedCollectionAsync = await singleCollectionProvider.GetCollectionAsync(null);

            //Assert
            Assert.AreEqual(mongoCollection, returnedCollection);
            Assert.AreEqual(mongoCollection, returnedCollectionAsync);
        }

        #endregion
    }
}
