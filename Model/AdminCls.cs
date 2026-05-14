using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WebApiNew.Model
{
    public class AdminCls
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public string? Id { get; set; }

        public string? AdminId { get; set; }

       
        public string AdminName { get; set; }
        public string? Role { get; set; }

        [Required]
        public string Qualification { get; set; }

        [Required]
        public string Experience { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string Mobile { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string? ImageBase64 { get; set; }


        public string? ImagePath { get; set; }
        [Required]
        public string Password { get; set; }

        [BsonIgnore]
        [JsonIgnore]
        public IFormFile? File { get; set; }


        [Required]
        public string Address { get; set; }
        public DateTime cDt { get; set; }
        public DateTime? uDt { get; set; }
    }
}
