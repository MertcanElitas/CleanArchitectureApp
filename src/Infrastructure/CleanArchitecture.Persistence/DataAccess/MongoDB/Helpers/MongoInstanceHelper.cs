using MongoDB.Driver;

namespace CleanArchitecture.Persistence.DataAccess.MongoDB.Helpers
{
    public class MongoInstanceHelper : IMongoInstanceHelper
    {
        private readonly IMongoDatabase _mongoDatabase;
        public  MongoClient mongoClient { get; set; }

        public MongoInstanceHelper(IDatabaseSettings databaseSettings)
        {
            mongoClient = new MongoClient(databaseSettings.ConnectionString);
            _mongoDatabase = mongoClient.GetDatabase(databaseSettings.DatabaseName);
        }

        public IMongoCollection<T> GetMongoCollectionByName<T>(string collectionName)
        {
            var mongoCollection = _mongoDatabase.GetCollection<T>(collectionName);

            return mongoCollection;
        }
    }
}