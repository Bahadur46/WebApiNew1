using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApiNew.Model
{
    public class StudentCls
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public string? Id { get; set; }

        public string? StudentId { get; set; }

       
        public string StudentName { get; set; }

        [Required]
        public DateTime DOB { get; set; }

       

        [Required]
        public string Gender { get; set; }

        [Required]
        public string Mobile { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string? ImageBase64 { get; set; }


        public string? ImagePath { get; set; }
        

        [BsonIgnore]
        [JsonIgnore]
        public IFormFile? File { get; set; }


        [Required]
        public string Address { get; set; }
        [Required]
        public string ParentName { get; set; }
        
      
        public DateTime cDt { get; set; }
        public DateTime? uDt { get; set; }
        public string Classes { get; set; }
        public List<string> Section { get; set; }
        public string Role { get;  set; }
    }
}
