/*
 * This model is used to manipulate the data in Schedules
 */
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Service.Models
{
    public class Schedule
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string? ScheduleId { get; set; }
        public string? Start { get; set; }
        public string? End { get; set; }
        public string? ArrivalTime { get; set; }
        public string? DepartureTime { get; set; }
        public string? Date { get; set; }
        public bool? IsReserved { get; set; }
    }
}
