/*
 * This model is used to manipulate the data in Users
 */
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Service.Models
{
    public class User
    {
        [BsonId]
        public string? NIC { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; } // can be office, agent, traveller
        public string? ContactNo { get; set; }
        public string? Name { get; set; }
        public string? Age { get; set; }
        public string? Address { get; set; }
        public bool? IsActive { get; set; }
        public List<string>? ReservationIds { get; set; }
    }
}
