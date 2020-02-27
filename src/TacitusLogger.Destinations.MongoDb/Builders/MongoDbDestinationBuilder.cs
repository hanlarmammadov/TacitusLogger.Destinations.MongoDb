using System; 
using TacitusLogger.Builders; 

namespace TacitusLogger.Destinations.MongoDb
{
    /// <summary>
    /// Builds and adds an instance of <c>MongoDbDestination</c> class to the <c>LogGroupDestinationsBuilder</c>
    /// </summary>
    public class MongoDbDestinationBuilder: IMongoDbDestinationBuilder
    {
        private readonly ILogGroupDestinationsBuilder _logGroupDestinationsBuilder;
        private IMongoCollectionProvider _collectionProvider;
        private IBsonDocumentBuilder _bsonDocumentBuilder;

        /// <summary>
        /// Creates an instance of <c>MongoDbDestinationBuilder</c> using parent <c>ILogGroupDestinationsBuilder</c> instance.
        /// </summary>
        /// <param name="logGroupDestinationsBuilder"></param>
        public MongoDbDestinationBuilder(ILogGroupDestinationsBuilder logGroupDestinationsBuilder)
        {
            _logGroupDestinationsBuilder = logGroupDestinationsBuilder;
        }

        /// <summary>
        /// Gets <c>ILogGroupDestinationsBuilder</c> specified during the initialization. 
        /// </summary>
        public ILogGroupDestinationsBuilder LogGroupDestinationsBuilder => _logGroupDestinationsBuilder;
        /// <summary>
        /// Gets collection provider specified during the build process. 
        /// </summary>
        public IMongoCollectionProvider CollectionProvider => _collectionProvider;
        /// <summary>
        /// Gets BSON builder specified during the build process.
        /// </summary>
        public IBsonDocumentBuilder BsonDocumentBuilder => _bsonDocumentBuilder;

        /// <summary>
        /// Adds an instance of <c>IMongoCollectionProvider</c> to the building <c>MongoDbDestination</c>.
        /// </summary>
        /// <exception cref="ArgumentNullException">If <paramref name="collectionProvider"/> is null.</exception>
        /// <param name="collectionProvider">Collection provider.</param>
        /// <returns></returns>
        public IMongoDbDestinationBuilder WithCollection(IMongoCollectionProvider collectionProvider)
        {
            if (_collectionProvider != null)
                throw new InvalidOperationException("Collection provider has already been provided");
            _collectionProvider = collectionProvider ?? throw new ArgumentNullException("collectionProvider");
            return this;
        }
        /// <summary>
        /// Adds an instance of <c>IBsonDocumentBuilder</c> to the building <c>MongoDbDestination</c>.
        /// </summary> 
        /// <exception cref="ArgumentNullException">If <paramref name="bsonDocumentBuilder"/> is null.</exception>
        /// <param name="bsonDocumentBuilder">BSON builder.</param>
        /// <returns></returns>
        public IMongoDbDestinationBuilder WithBsonDocument(IBsonDocumentBuilder bsonDocumentBuilder)
        {
            if (_bsonDocumentBuilder != null)
                throw new InvalidOperationException("BSON document builder has already been provided");
            _bsonDocumentBuilder = bsonDocumentBuilder ?? throw new ArgumentNullException("bsonDocumentBuilder");
            return this;
        }

        /// <summary>
        /// Completes log destination build process by adding it to the parent log group destination builder.
        /// </summary>
        /// <returns></returns>
        public ILogGroupDestinationsBuilder Add()
        {
            // MongoDbDestination does not have default collection provider that
            // is why it should always be specified explicitly.
            if (_collectionProvider == null)
                throw new Exception("Collection provider was not assigned to builder");

            if (_bsonDocumentBuilder == null)
                _bsonDocumentBuilder = new DirectBsonDocumentBuilder();

            // Create the destination.
            MongoDbDestination mongoDbDestination = new MongoDbDestination(_collectionProvider, _bsonDocumentBuilder);

            // Add to log group.
            return _logGroupDestinationsBuilder.CustomDestination(mongoDbDestination);
        }
    }
}
