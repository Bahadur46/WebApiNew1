using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class User
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("Name")]
    public string Name { get; set; }  // <-- Add this

    [BsonElement("Email")]
    public string Email { get; set; }
    public string Mobile { get; set; }

    [BsonElement("Password")]
    public string Password { get; set; }

    [BsonElement("Role")]
    public string Role { get; set; }

    [BsonElement("IsActive")]
    public bool IsActive { get; set; }
}
