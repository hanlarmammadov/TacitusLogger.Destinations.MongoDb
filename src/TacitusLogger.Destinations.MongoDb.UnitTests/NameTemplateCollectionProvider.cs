using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System; 
using System.Threading.Tasks; 

namespace TacitusLogger.Destinations.MongoDb.UnitTests
{
    [TestFixture]
    public class NameTemplateCollectionProviderTests
    {
        #region Ctor tests

        [Test]
        public void Ctor_Taking_Database_And_Name_Template_When_Called_Init_Database_And_Template_Properties()
        {
            //Arrange
            var mongoDatabase = new Mock<IMongoDatabase>().Object;
            string tempalte = "template1";

            //Act
            NameTemplateCollectionProvider nameTemplateBasedCollectionProvider = new NameTemplateCollectionProvider(mongoDatabase, tempalte);

            //Assert
            Assert.AreEqual(mongoDatabase, nameTemplateBasedCollectionProvider.Database);
            Assert.AreEqual(tempalte, nameTemplateBasedCollectionProvider.CollectionNameTemplate);
        }

        [Test]
        public void Ctor_Taking_Database_And_Name_Template_When_Called_With_Null_Database_Throws_ArgumentNullException()
        {
            //Arrange 
            string tempalte = "template1";

            //Assert
            ArgumentNullException ex = Assert.Catch<ArgumentNullException>(() =>
            {
                //Act
                NameTemplateCollectionProvider nameTemplateBasedCollectionProvider = new NameTemplateCollectionProvider(null, tempalte);
            });
        }

        [Test]
        public void Ctor_Taking_Database_And_Name_Template_When_Called_With_Null_Template_Throws_ArgumentNullException()
        {
            //Arrange
            var mongoDatabase = new Mock<IMongoDatabase>().Object;

            //Assert
            Assert.Catch<ArgumentNullException>(() =>
            {
                //Act
                NameTemplateCollectionProvider nameTemplateBasedCollectionProvider = new NameTemplateCollectionProvider(mongoDatabase, null);
            });
        }
         
        [Test]
        public void Ctor_Taking_Database_When_Called_With_Null_Database_Throws_ArgumentNullException()
        {
            //Assert
            Assert.Catch<ArgumentNullException>(() =>
            {
                //Act
                NameTemplateCollectionProvider nameTemplateBasedCollectionProvider = new NameTemplateCollectionProvider(null as IMongoDatabase, "template");
            });
        }

        #endregion

        #region Tests for GetCollection and GetCollectionAsync method

        [Test]
        public async Task GetCollection_When_Called_Returns_Collection_Obtained_From_Database()
        {
            //Arrange
            var mongoDatabaseMock = new Mock<IMongoDatabase>();
            var mongoCollection = new Mock<IMongoCollection<BsonDocument>>().Object;
            mongoDatabaseMock.Setup(x => x.GetCollection<BsonDocument>("templateWithoutPlaceholders", It.IsAny<MongoCollectionSettings>())).Returns(mongoCollection);

            NameTemplateCollectionProvider nameTemplateBasedCollectionProvider = new NameTemplateCollectionProvider(mongoDatabaseMock.Object, "templateWithoutPlaceholders");
            LogModel logModel = new LogModel() { };

            //Act
            var returnedCollection = nameTemplateBasedCollectionProvider.GetCollection(logModel);
            var returnedCollectionAsync = await nameTemplateBasedCollectionProvider.GetCollectionAsync(logModel);

            //Assert
            Assert.AreEqual(mongoCollection, returnedCollection);
            Assert.AreEqual(mongoCollection, returnedCollectionAsync);
        }

        [TestCase("templateWithoutPlaceholders", "templateWithoutPlaceholders")]
        [TestCase(".$@!#1234567890/,\\`~%^&*()", ".$@!#1234567890/,\\`~%^&*()")]
        [TestCase("$Source$Context$LogType$LogDate", "Source1Context1Error10-Dec-2019")]
        [TestCase("$Source(3)$Context(4)$LogType(1)$LogDate(dd.MM.yyyy)", "SouContE10.12.2019")]
        [TestCase("$Source(100)$Context(100)$LogType(100)$LogDate", "Source1Context1Error10-Dec-2019")]
        [TestCase("$Context", "Context1")]
        [TestCase("$Source", "Source1")]
        [TestCase("$LogType", "Error")]
        [TestCase("$LogDate", "10-Dec-2019")]
        //Log date with different formats 
        [TestCase("$LogDate(MM.dd.yyyy)", "12.10.2019")]
        //[TestCase(@"$LogDate(MM/dd/yyyy)", @"12/10/2019")]
        [TestCase("$LogDate(MM:dd:yyyy)", "12:10:2019")]
        [TestCase("$LogDate(MM.dd.yyyy hh.mm.ss)", "12.10.2019 07.08.10")]
        //Doubled $ before placeholder
        [TestCase("$$Context$", "$Context1$")]
        [TestCase("$$Source$", "$Source1$")]
        [TestCase("$$LogType$", "$Error$")]
        [TestCase("$$LogDate$", "$10-Dec-2019$")]
        //Empty parentheses after placeholder
        [TestCase("$Context()", "Context1()")]
        [TestCase("$Source()", "Source1()")]
        [TestCase("$LogType()", "Error()")]
        [TestCase("$LogDate()", "10-Dec-2019()")]
        //Absent placeholders
        [TestCase("$Description", "$Description")]
        [TestCase("$LoggingObject", "$LoggingObject")]
        [TestCase("$LogId", "$LogId")]
        public async Task GetCollection_When_Called_Passes_The_Right_String_To_Mongo_Database_Method(string providedTemplate, string expectedCollectionName)
        {
            //Arrange
            var mongoDatabase = new Mock<IMongoDatabase>();
            NameTemplateCollectionProvider nameTemplateBasedCollectionProvider = new NameTemplateCollectionProvider(mongoDatabase.Object, providedTemplate);
            LogModel logModel = new LogModel()
            {
                Source = "Source1",
                Context = "Context1",
                LogType = LogType.Error,
                LogDate = new DateTime(2019, 12, 10, 7, 8, 10, 1)
            };

            //Act
            nameTemplateBasedCollectionProvider.GetCollection(logModel);
            await nameTemplateBasedCollectionProvider.GetCollectionAsync(logModel);

            //Assert
            mongoDatabase.Verify(x => x.GetCollection<BsonDocument>(expectedCollectionName, null), Times.Exactly(2));
        }

        [TestCase("$Source$Context$LogType$LogDate", "Error10-Dec-2019")]
        [TestCase("$Source(3)$Context(4)$LogType(1)$LogDate(dd.MM.yyyy)", "E10.12.2019")]
        [TestCase("$Source(100)$Context(100)$LogType(100)$LogDate", "Error10-Dec-2019")]
        [TestCase("$Source$Context", "")]
        [TestCase("$Context", "")]
        [TestCase("$Source", "")]
        //Doubled $ before placeholder
        [TestCase("$$Context$", "$$")]
        [TestCase("$$Source$", "$$")]
        //Empty parentheses after placeholder
        [TestCase("$Context()", "()")]
        [TestCase("$Source()", "()")]
        public async Task GetCollection_When_Called_Given_That_Source_And_Context_Are_Null_Passes_The_Right_String_To_Mongo_Database_Method(string providedTemplate, string expectedCollectionName)
        {
            //Arrange
            var mongoDatabase = new Mock<IMongoDatabase>();
            NameTemplateCollectionProvider nameTemplateBasedCollectionProvider = new NameTemplateCollectionProvider(mongoDatabase.Object, providedTemplate);
            LogModel logModel = new LogModel()
            {
                Source = null,
                Context = null,
                LogType = LogType.Error,
                LogDate = new DateTime(2019, 12, 10, 7, 8, 10, 1)
            };

            //Act
            nameTemplateBasedCollectionProvider.GetCollection(logModel);
            await nameTemplateBasedCollectionProvider.GetCollectionAsync(logModel);

            //Assert
            mongoDatabase.Verify(x => x.GetCollection<BsonDocument>(expectedCollectionName, null), Times.Exactly(2));
        }

        [TestCase(LogType.Info, "$LogType", "Info")]
        [TestCase(LogType.Event, "$LogType", "Event")]
        [TestCase(LogType.Warning, "$LogType", "Warning")]
        [TestCase(LogType.Error, "$LogType", "Error")]
        [TestCase(LogType.Critical, "$LogType", "Critical")]
        public async Task GetCollection_When_Called_With_Different_LogType_Passes_The_Right_String_To_Mongo_Database_Method(LogType logType, string providedTemplate, string expectedCollectionName)
        {
            //Arrange
            var mongoDatabase = new Mock<IMongoDatabase>();
            NameTemplateCollectionProvider nameTemplateBasedCollectionProvider = new NameTemplateCollectionProvider(mongoDatabase.Object, providedTemplate);
            LogModel logModel = new LogModel() { LogType = logType };

            //Act
            nameTemplateBasedCollectionProvider.GetCollection(logModel);
            await nameTemplateBasedCollectionProvider.GetCollectionAsync(logModel);

            //Assert
            mongoDatabase.Verify(x => x.GetCollection<BsonDocument>(expectedCollectionName, null), Times.Exactly(2));
        }

        #endregion
    }
}
