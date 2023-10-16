/*
* This service class provide the service for usercontroller
*/
using Service.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Services
{
    public class UsersService
    {
        private readonly IMongoCollection<User> _usersCollection;

        /* The way of initiating mongodb and implementeing mongo collection were referred using microsoft documentation
        https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-7.0&tabs=visual-studio */
        // Constructor to initialize the MongoDB collection
        public UsersService(IOptions<TravelDbDatabaseSettings> TravelDbDatabaseSettings)
        {
            var mongoClient = new MongoClient(TravelDbDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                TravelDbDatabaseSettings.Value.DatabaseName
            );

            _usersCollection = mongoDatabase.GetCollection<User>(
                TravelDbDatabaseSettings.Value.UsersCollectionName
            );
        }

        // Retrieve all users asynchronously
        public async Task<List<User>> GetAsync() =>
            await _usersCollection.Find(_ => true).ToListAsync();

        // Retrieve a user by NIC (National Identity Card) asynchronously
        public async Task<User?> GetAsync(string NIC) =>
            await _usersCollection.Find(x => x.NIC == NIC).FirstOrDefaultAsync();

        // Create a new user asynchronously
        public async Task CreateAsync(User newUser) =>
            await _usersCollection.InsertOneAsync(newUser);

        // Update a user by NIC asynchronously
        public async Task UpdateAsync(string NIC, User updatedUser) =>
            await _usersCollection.ReplaceOneAsync(x => x.NIC == NIC, updatedUser);

        // Remove a user by NIC asynchronously
        public async Task RemoveAsync(string NIC) =>
            await _usersCollection.DeleteOneAsync(x => x.NIC == NIC);
    }
}
