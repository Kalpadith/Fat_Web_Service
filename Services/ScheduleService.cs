/*
* This service class provide the service to APIs in ScheduleController
*/
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Services
{
    public class ScheduleService
    {
        private readonly IMongoCollection<Schedule> _schedulesCollection;

        /* The way of initiating mongodb and implementeing mongo collection were referred using microsoft documentation
        https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-7.0&tabs=visual-studio */
        // Constructor for initializing the ScheduleService
        public ScheduleService(IOptions<TravelDbDatabaseSettings> TravelDbDatabaseSettings)
        {
            var mongoClient = new MongoClient(TravelDbDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                TravelDbDatabaseSettings.Value.DatabaseName
            );

            _schedulesCollection = mongoDatabase.GetCollection<Schedule>(
                TravelDbDatabaseSettings.Value.SchedulesCollectionName
            );
        }

        // Retrieves all schedules asynchronously
        public async Task<List<Schedule>> GetAsync() =>
            await _schedulesCollection.Find(_ => true).ToListAsync();

        // Retrieves a schedule by its ID asynchronously
        public async Task<Schedule?> GetAsync(string ScheduleId) =>
            await _schedulesCollection.Find(x => x.ScheduleId == ScheduleId).FirstOrDefaultAsync();

        // Creates a new schedule asynchronously
        public async Task CreateAsync(Schedule newSchedule) =>
            await _schedulesCollection.InsertOneAsync(newSchedule);

        // Updates an existing schedule by its ID asynchronously
        public async Task UpdateAsync(string ScheduleId, Schedule updatedSchedule) =>
            await _schedulesCollection.ReplaceOneAsync(
                x => x.ScheduleId == ScheduleId,
                updatedSchedule
            );

        // Removes a schedule by its ID asynchronously
        public async Task RemoveAsync(string ScheduleId) =>
            await _schedulesCollection.DeleteOneAsync(x => x.ScheduleId == ScheduleId);
    }
}
