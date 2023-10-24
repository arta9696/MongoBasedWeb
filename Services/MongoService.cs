using Microsoft.Extensions.Options;
using MongoBasedWeb.Data;
using MongoBasedWeb.Models;
using MongoDB.Driver;

namespace MongoBasedWeb.Services
{
    public class MongoService
    {
        IMongoDatabase _database;
        List<string> _collectionNames = new List<string>();
        public MongoService(IOptions<MongoBasedWebContext> context) {
            _database = new MongoClient(
                context.Value.ConnectionString)
                .GetDatabase(
                context.Value.DatabaseName);

            _collectionNames.Add(context.Value.CatsCollectionName);
            _collectionNames.Add(context.Value.AttributesCollectionName);
        }
        public IMongoCollection<T> GetCollection<T>()
        {
            return _database.GetCollection<T>(_collectionNames.Find(x=>x.Contains(typeof(T).Name)));
        }
    }
}
