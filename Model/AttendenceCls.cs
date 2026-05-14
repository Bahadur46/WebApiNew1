using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using WebApiNew.Model;

public class AttendanceCls {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string? StudentId { get; set; }
    public string? TeacherId { get; set; }
    public string? StudentName { get; set; }

    public string? Role { get; set; }

    public DateTime AttendanceDate { get; set; } = DateTime.UtcNow;

    public DateTime? PunchInTime { get; set; } = DateTime.UtcNow;

    public DateTime? CheckOutTime { get; set; } = DateTime.UtcNow;

    public string? Status { get; set; }

    public DateTime cDt { get; set; } = DateTime.UtcNow;
}
