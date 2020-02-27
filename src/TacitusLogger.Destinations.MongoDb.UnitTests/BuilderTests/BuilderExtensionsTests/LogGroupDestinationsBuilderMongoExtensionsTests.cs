using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TacitusLogger.Builders;

namespace TacitusLogger.Destinations.MongoDb.UnitTests.BuilderTests.BuilderExtensionsTests
{
    [TestFixture]
    public class LogGroupDestinationsBuilderMongoExtensionsTests
    {
        [Test]
        public void MongoDb_When_Called_Returns_New_MongoDbDestinationBuilder()
        {
            //Arrange
            var logGroupDestinationsBuilderMock = new Mock<LogGroupDestinationsBuilder>(null);

            //Act
            var mongoDbDestinationBuilder = LogGroupDestinationsBuilderMongoExtensions.MongoDb(logGroupDestinationsBuilderMock.Object);

            //Assert
            Assert.IsTrue(mongoDbDestinationBuilder is MongoDbDestinationBuilder);
            Assert.AreEqual(logGroupDestinationsBuilderMock.Object, (mongoDbDestinationBuilder as MongoDbDestinationBuilder).LogGroupDestinationsBuilder); 
        }
    }
}
