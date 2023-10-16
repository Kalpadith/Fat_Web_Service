/*
 * This model is used to manipulate the data in reservations
 */
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Service.Models
{
    public class Reservation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string? ReserveId { get; set; }
        public string? TrainId { get; set; }
        public string? ReferenceId { get; set; } //nic of agent
        public DateTime ReservationDate { get; set; }
        public DateTime ReservedDate { get; set; }
        public string? ReservedFor { get; set; } //Nic of the traveller
    }
}
