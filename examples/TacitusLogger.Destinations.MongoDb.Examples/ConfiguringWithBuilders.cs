using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using TacitusLogger.Builders;

namespace TacitusLogger.Destinations.MongoDb.Examples
{
    public class ConfiguringWithBuilders
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
            var logger = LoggerBuilder.Logger()
                                      .ForAllLogs()
                                      .MongoDb()
                                          .WithCollection(logsCollection)
                                          .Add()
                                      .BuildLogger();
        }
        public void Adding_MongoDb_Destination_With_Specified_Mongo_Database_And_Collection_Name_Template()
        { 
            var logger = LoggerBuilder.Logger()
                                      .ForAllLogs()
                                      .MongoDb()
                                          .WithCollection(mongoDatabase, "$LogType-logs")
                                          .Add()
                                      .BuildLogger();
        }
        public void Adding_MongoDb_Destination_With_Custom_Mongo_Collection_Provider()
        {
            IMongoCollectionProvider customMongoCollectionProvider = new Mock<IMongoCollectionProvider>().Object;

            var logger = LoggerBuilder.Logger()
                                      .ForAllLogs()
                                      .MongoDb()
                                          .WithCollection(customMongoCollectionProvider)
                                          .Add()
                                      .BuildLogger();
        }
        public void Adding_MongoDb_Destination_With_Collection_Factory_Method()
        {
            LogModelFunc<IMongoCollection<BsonDocument>> collectionFactoryMethod = (logModel) =>
            {
                if (logModel.LogTypeIsIn(LogType.Error, LogType.Failure, LogType.Critical))
                    return importantLogsCollection;
                else
                    return logsCollection;
            };

            var logger = LoggerBuilder.Logger()
                                      .ForAllLogs()
                                      .MongoDb()
                                          .WithCollection(collectionFactoryMethod)
                                          .Add()
                                      .BuildLogger();
        }
        public void Adding_MongoDb_Destination_With_Log_Collections_Dictionary()
        {
            Dictionary<LogType, IMongoCollection<BsonDocument>> logCollectionsDictionary = new Dictionary<LogType, IMongoCollection<BsonDocument>>()
            {
                {LogType.Info, infoLogsCollection },
                {LogType.Success, successLogsCollection },
                {LogType.Event, eventLogsCollection },
                {LogType.Warning, warningLogsCollection },
                {LogType.Error, errorLogsCollection },
                {LogType.Failure, failureLogsCollection },
                {LogType.Critical, criticalLogsCollection }
            };

            var logger = LoggerBuilder.Logger()
                                      .ForAllLogs()
                                      .MongoDb()
                                          .WithCollection(logCollectionsDictionary)
                                          .Add()
                                      .BuildLogger();
        }
        public void Adding_MongoDb_Destination_With_Log_Model_Bson_Document()
        {
            var logger = LoggerBuilder.Logger()
                                      .ForAllLogs()
                                      .MongoDb()
                                          .WithCollection(logsCollection)
                                          .WithLogModelBsonDocument()
                                          .Add()
                                      .BuildLogger();
        }
        public void Adding_MongoDb_Destination_With_Custom_Bson_Document_Builder() 
        {
            IBsonDocumentBuilder customBsonDocumentBuilder = new Mock<IBsonDocumentBuilder>().Object;

            var logger = LoggerBuilder.Logger()
                                      .ForAllLogs()
                                      .MongoDb()
                                          .WithCollection(logsCollection)
                                          .WithBsonDocument(customBsonDocumentBuilder)
                                          .Add()
                                      .BuildLogger();
        }
        public void Adding_MongoDb_Destination_With_Bson_Document_Generator_Function()
        {
            LogModelFunc<BsonDocument> bsonDocumentGeneratorFunction = (logModel) =>
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
            };

            var logger = LoggerBuilder.Logger() 
                                      .ForAllLogs()
                                      .MongoDb()
                                          .WithCollection(logsCollection)
                                          .WithBsonDocument(bsonDocumentGeneratorFunction)
                                          .Add()
                                      .BuildLogger();
        }
    }
}
