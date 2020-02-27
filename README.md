# TacitusLogger.Destinations.MongoDb

> Extension destination for TacitusLogger that sends logs to MongoDb database.
 
Dependencies:  
* .Net Standard >= 1.5  
* TacitusLogger >= 0.1.0
* MongoDB.Driver >= 2.3.0
  
> Attention: `TacitusLogger.Destinations.MongoDb` is currently in **Alpha phase**. This means you should not use it in any production code.

## Installation

The NuGet <a href="http://example.com/" target="_blank">package</a>:

```powershell
PM> Install-Package TacitusLogger.Destinations.MongoDb
```

## Examples

### Adding MongoDb destination with minimal configurations
Using builders:
```cs
var logger = LoggerBuilder.Logger()
                          .ForAllLogs()
                          .MongoDb()
                              .WithCollection(logsCollection)
                              .Add()
                          .BuildLogger();
```
Or directly:
```cs
IMongoCollectionProvider mongoCollectionProvider = new DirectCollectionProvider(logsCollection);
IBsonDocumentBuilder bsonDocumentBuilder = new DirectBsonDocumentBuilder();
MongoDbDestination mongoDbDestination = new MongoDbDestination(mongoCollectionProvider, bsonDocumentBuilder);
Logger logger = new Logger();
logger.AddLogDestinations(mongoDbDestination);
```
---
### With specified MongoDb database and collection name template
Using builders:
```cs
var logger = LoggerBuilder.Logger()
                          .ForAllLogs()
                          .MongoDb()
                              .WithCollection(mongoDatabase, "$LogType-logs")
                              .Add()
                          .BuildLogger();
```
Or directly:
```cs
IMongoCollectionProvider mongoCollectionProvider = new NameTemplateCollectionProvider(mongoDatabase, "$LogType-logs"); 
MongoDbDestination mongoDbDestination = new MongoDbDestination(mongoCollectionProvider, new DirectBsonDocumentBuilder());
Logger logger = new Logger();
logger.AddLogDestinations(mongoDbDestination);
```
---
### With custom MongoDb collection provider
Using builders:
```cs
IMongoCollectionProvider customMongoCollectionProvider = new Mock<IMongoCollectionProvider>().Object;

var logger = LoggerBuilder.Logger()
                          .ForAllLogs()
                          .MongoDb()
                              .WithCollection(customMongoCollectionProvider)
                              .Add()
                          .BuildLogger();
```
Or directly:
```cs
IMongoCollectionProvider customMongoCollectionProvider = new Mock<IMongoCollectionProvider>().Object; 
MongoDbDestination mongoDbDestination = new MongoDbDestination(customMongoCollectionProvider, new DirectBsonDocumentBuilder());
Logger logger = new Logger();
logger.AddLogDestinations(mongoDbDestination);
```
---
### With collection factory method
Using builders:
```cs
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
```
Or directly:
```cs
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
```
---
### With log collections dictionary
Using builders:
```cs
var logCollectionsDictionary = new Dictionary<LogType, IMongoCollection<BsonDocument>>()
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
```
Or directly:
```cs
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
```
---
### With log model BSON document
Using builders:
```cs
var logger = LoggerBuilder.Logger()
                          .ForAllLogs()
                          .MongoDb()
                              .WithCollection(logsCollection)
                              .WithLogModelBsonDocument()
                              .Add()
                          .BuildLogger();
``` 
---
### With custom BSON document builder
Using builders:
```cs
IBsonDocumentBuilder customBsonDocumentBuilder = new Mock<IBsonDocumentBuilder>().Object;

var logger = LoggerBuilder.Logger()
                          .ForAllLogs()
                          .MongoDb()
                              .WithCollection(logsCollection)
                              .WithBsonDocument(customBsonDocumentBuilder)
                              .Add()
                          .BuildLogger();
```
Or directly:
```cs
IBsonDocumentBuilder customBsonDocumentBuilder = new Mock<IBsonDocumentBuilder>().Object;  
MongoDbDestination mongoDbDestination = new MongoDbDestination(new DirectCollectionProvider(logsCollection), customBsonDocumentBuilder);
Logger logger = new Logger();
logger.AddLogDestinations(mongoDbDestination);
```
---
### With BSON document generator function
Using builders:
```cs
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
```
Or directly:
```cs
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
```