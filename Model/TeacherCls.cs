using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class TeacherCreateDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore] 
    public string? Id { get; set; }
  
    public string? TeacherId { get; set; }  
 
    public int ?EmployeeId { get; set; }    
    [Required]
    public string TeacherName { get; set; }
    public string ?Role { get; set; }

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
    public DateTime ?uDt { get; set; }
    public List<string> Classes { get; set; }
    public List<string> Subjects { get; set; }
    public List<string> AssignedClass { get; set; }
}
