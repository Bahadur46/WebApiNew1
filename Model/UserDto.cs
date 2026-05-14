using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class UserDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public string? Id { get; set; }

   
  
    public string Name { get; set; }
    public string Email { get; set; }
    public string Mobile { get; set; }
    public string Role { get; set; }
    public string Token { get; set; } // if using JWT
    public string Password { get;  set; }
}
