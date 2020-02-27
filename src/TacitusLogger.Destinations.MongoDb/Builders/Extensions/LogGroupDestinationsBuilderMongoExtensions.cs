using TacitusLogger.Builders;

namespace TacitusLogger.Destinations.MongoDb
{
    public static class LogGroupDestinationsBuilderMongoExtensions
    {
        public static IMongoDbDestinationBuilder MongoDb(this ILogGroupDestinationsBuilder obj)
        {
            return new MongoDbDestinationBuilder(obj);
        }
    }
}
