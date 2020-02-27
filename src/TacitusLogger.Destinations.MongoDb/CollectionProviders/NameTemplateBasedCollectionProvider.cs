using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TacitusLogger.Components.TemplateResolving;
using TacitusLogger.Components.TemplateResolving.PlaceholderResolvers;
using TacitusLogger.Serializers;

namespace TacitusLogger.Destinations.MongoDb
{
    /// <summary>
    /// Implementation of <c>IMongoCollectionProvider</c> that selects a collection 
    /// using special collection name template string. 
    /// </summary>
    public class NameTemplateCollectionProvider : SynchronousCollectionProviderBase
    {
        private static readonly string _defaultDateFormat = Templates.DateFormats.DefaultFileName;
        private readonly string _collectionNameTemplate;
        private readonly IMongoDatabase _mongoDatabase;
        private LogModelTemplateResolver _logModelTemplateResolver;

        /// <summary>
        /// Initializes a new instance of <c>CollectionNameTemplateCollectionProvider</c> class using specified mongo database and collection name template.
        /// </summary>
        /// <param name="mongoDatabase">Mongo database</param>
        /// <param name="collectionNameTemplate">Collection name template that will be used to get the right collection.</param> 
        public NameTemplateCollectionProvider(IMongoDatabase mongoDatabase, string collectionNameTemplate)
        {
            _mongoDatabase = mongoDatabase ?? throw new ArgumentNullException("mongoDatabase");
            _collectionNameTemplate = collectionNameTemplate ?? throw new ArgumentNullException("collectionNameTemplate");

            // Create placeholder resolvers list according to permitted placeholders for this serializer and pass it to LogDataTemplateResolver.
            List<IPlaceholderResolver> placeholderResolvers = new List<IPlaceholderResolver>();
            placeholderResolvers.Add(new SourcePlaceholderResolver());
            placeholderResolvers.Add(new ContextPlaceholderResolver());
            placeholderResolvers.Add(new LogTypePlaceholderResolver());
            placeholderResolvers.Add(new LogDatePlaceholderResolver(_defaultDateFormat));
            _logModelTemplateResolver = new LogModelTemplateResolver(placeholderResolvers);
        }

        /// <summary>
        /// Gets the default date format.
        /// </summary>
        public static string DefaultLogDateFormat => _defaultDateFormat;
        /// <summary>
        /// Gets collection name template specified during initialization.
        /// </summary>
        public string CollectionNameTemplate => _collectionNameTemplate;
        /// <summary>
        /// Gets Mongo database specified during initialization.
        /// </summary>
        public IMongoDatabase Database => _mongoDatabase;

        /// <summary>
        /// Retrieves the according collection using input data.
        /// </summary>
        /// <param name="logModel"></param> 
        /// <returns>Resulting Mongo collection.</returns>
        public override IMongoCollection<BsonDocument> GetCollection(LogModel logModel)
        {
            // Generate collection name using log model and template. 
            string collectionName = _logModelTemplateResolver.Resolve(logModel, _collectionNameTemplate);
            // Get collection from db. If collection does not exists mongo will create a new one.
            return _mongoDatabase.GetCollection<BsonDocument>(collectionName);
        }
    }
}
