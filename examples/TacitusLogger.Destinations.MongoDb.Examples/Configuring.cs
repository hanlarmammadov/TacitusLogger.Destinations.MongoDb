using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using TacitusLogger.Builders;

namespace TacitusLogger.Destinations.MongoDb.Examples
{
    public class Configuring
    {
        private IMongoDatabase mongoDatabase;
        private IMongoCollection<BsonDocument> logsCollection;
        private IMongoCollection<BsonDocument> importantLogsCollection;

        private IMongoCollection<BsonDocument> infoLogsCollection;
        private IMongoCollection<BsonDocument> successLogsCollection;
        private IMongoCollection<BsonDocument> eventLogsCollection;
        private IMongoCollection<BsonDocument> warningLogsCollection;
        private IMongoCollection<BsonDocument> errorLogsCollection;
        private IMongoCollection<BsonDocument> failureLogsCollection;
        private IMongoCollection<BsonDocument> criticalLogsCollection;

        public void Adding_MongoDb_Destination_With_Minimal_Configurations()
        {
            IMongoCollectionProvider mongoCollectionProvider = new DirectCollectionProvider(logsCollection);
            IBsonDocumentBuilder bsonDocumentBuilder = new DirectBsonDocumentBuilder();
            MongoDbDestination mongoDbDestination = new MongoDbDestination(mongoCollectionProvider, bsonDocumentBuilder);
            Logger logger = new Logger();
            logger.AddLogDestinations(mongoDbDestination);
        }
        public void Adding_MongoDb_Destination_With_Specified_Mongo_Database_And_Collection_Name_Template()
        {
            IMongoCollectionProvider mongoCollectionProvider = new NameTemplateCollectionProvider(mongoDatabase, "$LogType-logs"); 
            MongoDbDestination mongoDbDestination = new MongoDbDestination(mongoCollectionProvider, new DirectBsonDocumentBuilder());
            Logger logger = new Logger();
            logger.AddLogDestinations(mongoDbDestination);
        }
        public void Adding_MongoDb_Destination_With_Custom_Mongo_Collection_Provider()
        {
            IMongoCollectionProvider customMongoCollectionProvider = new Mock<IMongoCollectionProvider>().Object; 
            MongoDbDestination mongoDbDestination = new MongoDbDestination(customMongoCollectionProvider, new DirectBsonDocumentBuilder());
            Logger logger = new Logger();
            logger.AddLogDestinations(mongoDbDestination);
        }
        public void Adding_MongoDb_Destination_With_Collection_Factory_Method()
        {
            GeneratorFunctionCollectionProvider factoryMethodCollectionProvider = new GeneratorFunctionCollectionProvider((logModel) =>
            {
                if (logModel.LogTypeIsIn(LogType.Error, LogType.Failure, LogType.Critical))
                    return importantLogsCollection;
                else
                    return logsCollection;
            }); 
            MongoDbDestination mongoDbDestination = new MongoDbDestination(factoryMethodCollectionProvider, new DirectBsonDocumentBuilder());
            Logger logger = new Logger();
            logger.AddLogDestinations(mongoDbDestination);
        }
        public void Adding_MongoDb_Destination_With_Log_Collections_Dictionary()
        {
            LogTypeBasedCollectionProvider logTypeBasedCollectionProvider = new LogTypeBasedCollectionProvider(new Dictionary<LogType, IMongoCollection<BsonDocument>>()
            {
                {LogType.Info, infoLogsCollection },
                {LogType.Success, successLogsCollection },
                {LogType.Event, eventLogsCollection },
                {LogType.Warning, warningLogsCollection },
                {LogType.Error, errorLogsCollection },
                {LogType.Failure, failureLogsCollection },
                {LogType.Critical, criticalLogsCollection }
            });

            MongoDbDestination mongoDbDestination = new MongoDbDestination(logTypeBasedCollectionProvider, new DirectBsonDocumentBuilder());
            Logger logger = new Logger();
            logger.AddLogDestinations(mongoDbDestination);
        }
        public void Adding_MongoDb_Destination_With_Custom_Bson_Document_Builder()
        {
            IBsonDocumentBuilder customBsonDocumentBuilder = new Mock<IBsonDocumentBuilder>().Object;  
            MongoDbDestination mongoDbDestination = new MongoDbDestination(new DirectCollectionProvider(logsCollection), customBsonDocumentBuilder);
            Logger logger = new Logger();
            logger.AddLogDestinations(mongoDbDestination);
        }
        public void Adding_MongoDb_Destination_With_Bson_Document_Generator_Function()
        {
            ConverterFunctionBsonDocumentBuilder generatorFunctionBsonDocumentBuilder = new ConverterFunctionBsonDocumentBuilder((logModel) =>
            {
                var logDb = new
                {
                    id = logModel.LogId,
                    ctx = logModel.Context,
                    tags = logModel.Tags,
                    src = logModel.Source,
                    type = logModel.LogType,
                    desc = logModel.Description,
                    items = logModel.LogItems,
                    date = logModel.LogDate,
                };
                return logDb.ToBsonDocument();
            });

            MongoDbDestination mongoDbDestination = new MongoDbDestination(new DirectCollectionProvider(logsCollection), generatorFunctionBsonDocumentBuilder);
            Logger logger = new Logger();
            logger.AddLogDestinations(mongoDbDestination);
        }
    }
}
