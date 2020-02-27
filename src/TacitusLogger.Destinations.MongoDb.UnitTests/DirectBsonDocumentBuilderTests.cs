using MongoDB.Bson;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TacitusLogger.Destinations.MongoDb.UnitTests
{
    [TestFixture]
    public class DirectBsonDocumentBuilderTests
    {
        #region Tests for BuildFrom and BuildFromAsync methods

        [Test]
        public void BuildFrom_When_Called_With_LogData_Returns_BsonDocument_That_Reflects_Provided_LogModel()
        {
            //Arrange
            DirectBsonDocumentBuilder directBsonDocumentBuilder = new DirectBsonDocumentBuilder();
            LogModel logModel = new LogModel()
            {
                Context = "Context1"
            };

            //Act
            BsonDocument bsonDoc = directBsonDocumentBuilder.BuildFrom(logModel);

            //Assert
            string jsonFromSource = logModel.ToJson();
            string jsonFromResult = bsonDoc.ToJson();
            Assert.AreEqual(jsonFromSource, jsonFromResult);
        }
        
        [Test]
        public void BuildFrom_When_Called_With_Null_LogData_Throws_ArgumentNullException()
        {
            //Arrange
            DirectBsonDocumentBuilder directBsonDocumentBuilder = new DirectBsonDocumentBuilder();

            //Assert 
            Assert.Catch<ArgumentNullException>(() =>
            {
                //Act
                BsonDocument bsonDoc = directBsonDocumentBuilder.BuildFrom(null);
            });
        }
        
        #endregion

        #region Tests for BuildFromAsync methods
         
        [Test]
        public async Task BuildFromAsync_When_Called_With_LogData_Returns_BsonDocument_That_Reflects_Provided_LogModel()
        {
            //Arrange
            DirectBsonDocumentBuilder directBsonDocumentBuilder = new DirectBsonDocumentBuilder();
            LogModel logModel = new LogModel()
            {
                Context = "Context1"
            };

            //Act 
            BsonDocument bsonDocAsync = await directBsonDocumentBuilder.BuildFromAsync(logModel);

            //Assert
            string jsonFromSource = logModel.ToJson();
            string jsonFromResult = bsonDocAsync.ToJson();
            Assert.AreEqual(jsonFromSource, jsonFromResult);
        }

        [Test]
        public void BuildFromAsync_When_Called_With_Null_LogData_Throws_ArgumentNullException()
        {
            //Arrange
            DirectBsonDocumentBuilder directBsonDocumentBuilder = new DirectBsonDocumentBuilder();

            //Assert 
            Assert.CatchAsync<ArgumentNullException>(async () =>
            {
                //Act
                BsonDocument bsonDoc = await directBsonDocumentBuilder.BuildFromAsync(null);
            });
        }

        #endregion
    }
}
