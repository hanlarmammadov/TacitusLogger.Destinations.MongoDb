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
    public class LogTypeBasedCollectionProviderTests
    {
        protected Dictionary<LogType, IMongoCollection<BsonDocument>> GetSampleDictionary()
        {
            return new Dictionary<LogType, IMongoCollection<BsonDocument>>()
            {
                {LogType.Success, new Mock<IMongoCollection<BsonDocument>>().Object  },
                {LogType.Info, new Mock<IMongoCollection<BsonDocument>>().Object  },
                {LogType.Event, new Mock<IMongoCollection<BsonDocument>>().Object  },
                {LogType.Warning, new Mock<IMongoCollection<BsonDocument>>().Object  },
                {LogType.Failure, new Mock<IMongoCollection<BsonDocument>>().Object  },
                {LogType.Error, new Mock<IMongoCollection<BsonDocument>>().Object  },
                {LogType.Critical, new Mock<IMongoCollection<BsonDocument>>().Object  },
            };
        }

        #region Ctor tests

        [Test]
        public void Ctor_When_Called_Sets_The_Dictionary()
        {
            //Arrange 
            Dictionary<LogType, IMongoCollection<BsonDocument>> dictionary = GetSampleDictionary();

            //Act
            LogTypeBasedCollectionProvider logTypeBasedCollectionProvider = new LogTypeBasedCollectionProvider(dictionary);

            //Assert
            Assert.AreEqual(dictionary, logTypeBasedCollectionProvider.Dictionary);
        }

        [Test]
        public void Ctor_When_Called_With_Null_Dictionary_Throws_ArgumentNullException()
        {
            //Assert
            Assert.Catch<ArgumentNullException>(() =>
            {
                //Act
                LogTypeBasedCollectionProvider logTypeBasedCollectionProvider = new LogTypeBasedCollectionProvider(null);
            });
        }

        [Test]
        public void Ctor_When_Called_With_Dictionary_That_Contains_Not_All_Log_Types_Throws_ArgumentException()
        {
            //Arrange 
            Dictionary<LogType, IMongoCollection<BsonDocument>> dictionary = GetSampleDictionary();
            dictionary.Remove(LogType.Event);

            //Assert
            Assert.Catch<ArgumentException>(() =>
            {
                //Act
                LogTypeBasedCollectionProvider logTypeBasedCollectionProvider = new LogTypeBasedCollectionProvider(dictionary);
            });
        }

        [Test]
        public void Ctor_When_Called_With_Dictionary_That_Contains_Null_Values_Throws_ArgumentException()
        {
            //Arrange 
            Dictionary<LogType, IMongoCollection<BsonDocument>> dictionary = GetSampleDictionary();
            dictionary[LogType.Error] = null;

            //Assert
            Assert.Catch<ArgumentException>(() =>
            {
                //Act
                LogTypeBasedCollectionProvider logTypeBasedCollectionProvider = new LogTypeBasedCollectionProvider(dictionary);
            });
        }

        #endregion

        #region Tests for GetCollection method

        [TestCase(LogType.Info)]
        [TestCase(LogType.Event)]
        [TestCase(LogType.Warning)]
        [TestCase(LogType.Error)]
        [TestCase(LogType.Critical)]
        public void GetCollection_When_Called_Returns_Collection_According_To_LogType(LogType logType)
        {
            //Arrange 
            Dictionary<LogType, IMongoCollection<BsonDocument>> dictionary = GetSampleDictionary();
            LogTypeBasedCollectionProvider logTypeBasedCollectionProvider = new LogTypeBasedCollectionProvider(dictionary);
            LogModel logModel = new LogModel() { LogType = logType };

            //Act
            var collectionReturned = logTypeBasedCollectionProvider.GetCollection(logModel);

            //Assert
            Assert.AreEqual(dictionary[logType], collectionReturned);
        }

        [Test]
        public void GetCollection_When_Called_With_Null_LogData_Throws_ArgumentNullException()
        {
            //Arrange 
            Dictionary<LogType, IMongoCollection<BsonDocument>> dictionary = GetSampleDictionary();
            LogTypeBasedCollectionProvider logTypeBasedCollectionProvider = new LogTypeBasedCollectionProvider(dictionary);

            //Assert
            Assert.Catch<ArgumentNullException>(() =>
            {
                //Act
                logTypeBasedCollectionProvider.GetCollection(null);
            });
        }

        #endregion

        #region Tests for GetCollectionAsync method

        [TestCase(LogType.Info)]
        [TestCase(LogType.Event)]
        [TestCase(LogType.Warning)]
        [TestCase(LogType.Error)]
        [TestCase(LogType.Critical)]
        public async Task GetCollectionAsync_When_Called_Returns_Collection_According_To_LogType(LogType logType)
        {
            //Arrange 
            Dictionary<LogType, IMongoCollection<BsonDocument>> dictionary = GetSampleDictionary();
            LogTypeBasedCollectionProvider logTypeBasedCollectionProvider = new LogTypeBasedCollectionProvider(dictionary);
            LogModel logModel = new LogModel() { LogType = logType };

            //Act
            var collectionReturned = await logTypeBasedCollectionProvider.GetCollectionAsync(logModel);

            //Assert
            Assert.AreEqual(dictionary[logType], collectionReturned);
        }

        [Test]
        public void GetCollectionAsync_When_Called_With_Null_LogData_Throws_ArgumentNullException()
        {
            //Arrange 
            Dictionary<LogType, IMongoCollection<BsonDocument>> dictionary = GetSampleDictionary();
            LogTypeBasedCollectionProvider logTypeBasedCollectionProvider = new LogTypeBasedCollectionProvider(dictionary);

            //Assert
            Assert.CatchAsync<ArgumentNullException>(async () =>
            {
                //Act
                await logTypeBasedCollectionProvider.GetCollectionAsync(null);
            });
        }

        #endregion
    }
}
