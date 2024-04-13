using Interfaces;
using MongoDB.Driver;
using MongoDB.Bson;
using Models;

namespace Data
{
    // Represents the MongoDB context for interacting with the database
    public class MongoDBContext : IMongoDBContext
    {
        private readonly IMongoCollection<BsonDocument> _collection;

        // Initializes a new instance of the MongoDBContext class
        public MongoDBContext(IMongoClient mongoClient)
        {
            string databaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME");
            string collectionName = Environment.GetEnvironmentVariable("MONGODB_COLLECTION_NAME");


            // Get the database and collection from the MongoDB client
            var database = mongoClient.GetDatabase(databaseName);
            _collection = database.GetCollection<BsonDocument>(collectionName);
        }

        // Inserts a message into the MongoDB collection asynchronously
        public async Task InsertMessageAsync(Message message)
        {
            _collection.InsertOne(message.ToBsonDocument());
        }
    }
}
