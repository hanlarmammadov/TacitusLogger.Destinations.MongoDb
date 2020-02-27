using MongoDB.Bson;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TacitusLogger.Destinations.MongoDb.UnitTests
{
    [TestFixture]
    public class ConverterFunctionBsonDocumentBuilderTests
    {
        #region Ctor tests

        [Test]
        public void Ctor_Taking_Factory_Method_When_Called_Sets_The_Specified_Factory_Method()
        {
            //Arrange
            LogModelFunc<BsonDocument> converterFunction = d => d.ToBsonDocument();
            ConverterFunctionBsonDocumentBuilder generatorFunctionBsonDocumentBuilder = new ConverterFunctionBsonDocumentBuilder(converterFunction);
            LogModel logModel = new LogModel()
            {
                Context = "Context1"
            };

            //Act
            BsonDocument bsonDoc = generatorFunctionBsonDocumentBuilder.BuildFrom(logModel);

            //Assert
            string jsonFromSource = logModel.ToJson();
            string jsonFromResult = bsonDoc.ToJson();
            Assert.AreEqual(jsonFromSource, jsonFromResult);
        }

        [Test]
        public void Ctor_Taking_Factory_Method_When_Called_With_Null_Delegate_Throws_ArgumentNullException()
        {
            //Assert
            Assert.Catch<ArgumentNullException>(() =>
            {
                //Act
                ConverterFunctionBsonDocumentBuilder generatorFunctionBsonDocumentBuilder = new ConverterFunctionBsonDocumentBuilder(null);
            });
        }

        #endregion

        #region Tests for BuildFrom method

        [Test]
        public void BuildFrom_When_Called_Builds_BsonDocument_Using_Factory_Method()
        {
            //Arrange
            LogModelFunc<BsonDocument> converterFunction = d => d.ToBsonDocument();
            ConverterFunctionBsonDocumentBuilder generatorFunctionBsonDocumentBuilder = new ConverterFunctionBsonDocumentBuilder(converterFunction);
            LogModel logModel = new LogModel()
            {
                Context = "Context1"
            };

            //Act
            BsonDocument bsonDoc = generatorFunctionBsonDocumentBuilder.BuildFrom(logModel);
            BsonDocument bsonUsingDelegate = converterFunction(logModel);

            //Assert 
            Assert.AreEqual(bsonUsingDelegate.ToJson(), bsonDoc.ToJson());
        }

        #endregion

        #region Tests for BuildFromAsync method

        [Test]
        public async Task BuildFromAsync_When_Called_Builds_BsonDocument_Using_Factory_Method()
        {
            //Arrange
            LogModelFunc<BsonDocument> converterFunction = d => d.ToBsonDocument();
            ConverterFunctionBsonDocumentBuilder generatorFunctionBsonDocumentBuilder = new ConverterFunctionBsonDocumentBuilder(converterFunction);
            LogModel logModel = new LogModel()
            {
                Context = "Context1"
            };

            //Act
            BsonDocument bsonDoc = await generatorFunctionBsonDocumentBuilder.BuildFromAsync(logModel);
            BsonDocument bsonUsingDelegate = converterFunction(logModel);

            //Assert 
            Assert.AreEqual(bsonUsingDelegate.ToJson(), bsonDoc.ToJson());
        }

        #endregion
    }
}
