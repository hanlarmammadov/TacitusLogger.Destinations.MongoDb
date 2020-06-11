# TacitusLogger.Destinations.MongoDb

> Extension destination for TacitusLogger that sends logs to MongoDb database.
 
Dependencies:  
* .Net Standard >= 1.5  
* TacitusLogger >= 0.1.0
* MongoDB.Driver >= 2.3.0
  
> Attention: `TacitusLogger.Destinations.MongoDb` is currently in **Alpha phase**. This means you should not use it in any production code.

## Installation

The NuGet <a href="https://www.nuget.org/packages/TacitusLogger.Destinations.MongoDb" target="_blank">package</a>:

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

## License

[APACHE LICENSE 2.0](https://www.apache.org/licenses/LICENSE-2.0)

## See also

TacitusLogger:  

- [TacitusLogger](https://github.com/khanlarmammadov/TacitusLogger) - A simple yet powerful .NET logging library.

Destinations:

- [TacitusLogger.Destinations.RabbitMq](https://github.com/khanlarmammadov/TacitusLogger.Destinations.RabbitMq) - Extension destination for TacitusLogger that sends logs to the RabbitMQ exchanges.
- [TacitusLogger.Destinations.Email](https://github.com/khanlarmammadov/TacitusLogger.Destinations.Email) - Extension destination for TacitusLogger that sends logs as emails using SMTP protocol.
- [TacitusLogger.Destinations.EntityFramework](https://github.com/khanlarmammadov/TacitusLogger.Destinations.EntityFramework) - Extension destination for TacitusLogger that sends logs to database using Entity Framework ORM.
- [TacitusLogger.Destinations.Trace](https://github.com/khanlarmammadov/TacitusLogger.Destinations.Trace) - Extension destination for TacitusLogger that sends logs to System.Diagnostics.Trace listeners.  
  
Dependency injection:
- [TacitusLogger.DI.Ninject](https://github.com/khanlarmammadov/TacitusLogger.DI.Ninject) - Extension for Ninject dependency injection container that helps to configure and add TacitusLogger as a singleton.
- [TacitusLogger.DI.Autofac](https://github.com/khanlarmammadov/TacitusLogger.DI.Autofac) - Extension for Autofac dependency injection container that helps to configure and add TacitusLogger as a singleton.
- [TacitusLogger.DI.MicrosoftDI](https://github.com/khanlarmammadov/TacitusLogger.DI.MicrosoftDI) - Extension for Microsoft dependency injection container that helps to configure and add TacitusLogger as a singleton.  

Log contributors:

- [TacitusLogger.Contributors.ThreadInfo](https://github.com/khanlarmammadov/TacitusLogger.Contributors.ThreadInfo) - Extension contributor for TacitusLogger that generates additional info related to the thread on which the logger method was called.
- [TacitusLogger.Contributors.MachineInfo](https://github.com/khanlarmammadov/TacitusLogger.Contributors.MachineInfo) - Extension contributor for TacitusLogger that generates additional info related to the machine on which the log was produced.