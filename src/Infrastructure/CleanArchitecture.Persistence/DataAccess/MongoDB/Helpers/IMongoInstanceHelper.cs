using MongoDB.Driver;

namespace CleanArchitecture.Persistence.DataAccess.MongoDB.Helpers
{
    public interface IMongoInstanceHelper
    {
        MongoClient mongoClient { get; set; }
        IMongoCollection<T> GetMongoCollectionByName<T>(string collectionName);
    }
}