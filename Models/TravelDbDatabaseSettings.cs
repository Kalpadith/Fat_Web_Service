/*
 * This model is used configure the databasesettings and collection settings
 */
namespace Service.Models
{
    public class TravelDbDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string UsersCollectionName { get; set; } = null!;
        public string TravellersCollectionName { get; set; } = null!;
        public string ReservationsCollectionName { get; set; } = null!;
        public string SchedulesCollectionName { get; set; } = null!;
    }
}
