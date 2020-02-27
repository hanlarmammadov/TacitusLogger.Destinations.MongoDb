using NUnit.Framework;
using System;
using TacitusLogger.Destinations.MongoDb.BsonDocumentBuilders;

namespace TacitusLogger.Destinations.MongoDb.UnitTests
{
    [TestFixture]
    public class BsonSerializableLogModelTests
    {
        [Test]
        public void Ctor_Taking_LogModel_When_Called_Sets_Fields()
        {
            // Arrange
            LogModel logModel = new LogModel()
            {
                Context = "Context1",
                Description = "Description1",
                LogId = "LogId1",
                Source = "Source1",
                LogType = LogType.Failure,
                Tags = new string[] { "tag1", "tag2", "tag3" },
                LogItems = new LogItem[] { new LogItem("item1", new { }), new LogItem("item2", new { }) },
                LogDate = DateTime.Now
            };

            // Act
            BsonSerializableLogModel bsonSerializableLogModel = new BsonSerializableLogModel(logModel);

            // Assert
            Assert.AreEqual(logModel.Context, bsonSerializableLogModel.Context);
            Assert.AreEqual(logModel.Description, bsonSerializableLogModel.Description);
            Assert.AreEqual(logModel.LogId, bsonSerializableLogModel.LogId);
            Assert.AreEqual(logModel.Source, bsonSerializableLogModel.Source);
            Assert.AreEqual(logModel.LogType, bsonSerializableLogModel.LogType);
            Assert.AreEqual(logModel.Tags, bsonSerializableLogModel.Tags);
            Assert.AreEqual(logModel.LogItems, bsonSerializableLogModel.LogItems);
            Assert.AreEqual(logModel.LogDate, bsonSerializableLogModel.LogDate);
        }
        [Test]
        public void Ctor_Taking_LogModel_When_Called_With_Null_Log_Model_Sets_Defaults()
        {
            // Act
            BsonSerializableLogModel bsonSerializableLogModel = new BsonSerializableLogModel(null as LogModel);

            // Assert
            Assert.IsNull(bsonSerializableLogModel.Context);
            Assert.IsNull(bsonSerializableLogModel.Description);
            Assert.IsNull(bsonSerializableLogModel.LogId);
            Assert.IsNull(bsonSerializableLogModel.Source);
            Assert.AreEqual(default(LogType), bsonSerializableLogModel.LogType);
            Assert.IsNull(bsonSerializableLogModel.Tags);
            Assert.IsNull(bsonSerializableLogModel.LogItems);
            Assert.AreEqual(default(DateTime), bsonSerializableLogModel.LogDate);
        }
    }
}
