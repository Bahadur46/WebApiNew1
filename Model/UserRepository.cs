using MongoDB.Bson;
using MongoDB.Driver;
using Sitecore.FakeDb;
using System.Reflection;
using WebApiNew.Model;
using static Org.BouncyCastle.Math.EC.ECCurve;
using static System.Collections.Specialized.BitVector32;

namespace WebApiNew.Repository
{
    public class UserRepository
    {
        private readonly IMongoCollection<User> _users;

        // 🔑 Constructor with IConfiguration
        public UserRepository(IConfiguration config)
        {
            // MongoDB connection string from appsettings.json
            var connStr = config.GetConnectionString("MongoDB"); // ya config["MongoDB:Connection"]
            if (string.IsNullOrEmpty(connStr))
                throw new Exception("MongoDB connection string missing in configuration!");

            var databaseName = config["MongoDB:Database"] ?? "LoginDB";

            var client = new MongoClient(connStr);
            var database = client.GetDatabase(databaseName);

            // Collection name must match DB
            _users = database.GetCollection<User>("LoginTB");
        }

        // 🔹 Remove unused constructor
        // public UserRepository(string? connStr, string v) { } // not needed

        // 🔹 Get any user by email
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            return await _users.Find(u => u.Email == email && u.IsActive)
                               .FirstOrDefaultAsync();
        }

        
        public async Task<User?> GetTeacherByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            return await _users.Find(u => u.Email == email && u.Role == "Teacher" && u.IsActive).FirstOrDefaultAsync();
        }

        // 🔹 Insert user
        public async Task InsertUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            await _users.InsertOneAsync(user);
        }

        public async Task<object?> Login(TeacherCreateDto model)
        {
            var mongoSettings = MongoClientSettings
                .FromConnectionString(Connection.getAttendanceConnection());

            var client = new MongoClient(mongoSettings);
            var database = client.GetDatabase("DHs_4001");

            var input = model.Email; // email ya mobile dono

            bool isEmail = input.Contains("@");

            // ================= STUDENT LOGIN =================
            if (model.Role == "student")
            {
                var collectionSt = database.GetCollection<StudentCls>("StudentTB");

                var builder = Builders<StudentCls>.Filter;

                var loginFilter = isEmail
                    ? builder.Eq(x => x.Email, input)
                    : builder.Eq(x => x.Mobile, input);

                var finalFilter = builder.And(
                    loginFilter,
                    builder.Eq(x => x.Password, model.Password),
                    builder.Eq(x => x.Role, model.Role)
                );

                var user = await collectionSt.Find(finalFilter).FirstOrDefaultAsync();

                if (user == null)
                    return null;

                return new
                {
                    Name = user.StudentName,
                    StudentId=user.StudentId,
                    DOB=user.DOB,
                    ParentName=user.ParentName,
                    cDt=user.cDt,
                    Email = user.Email,
                    Role = user.Role,
                    Mobile = user.Mobile,
                    Address = user.Address,
                    Class=user.Classes,
                    Section=user.Section,
                    ImagePath=user.ImagePath


                };
            }

            // ================= TEACHER LOGIN =================
            else if (model.Role == "teacher")
            {
                var collection = database.GetCollection<TeacherCreateDto>("TeacherTB");

                var builder = Builders<TeacherCreateDto>.Filter;

                var loginFilter = isEmail
                    ? builder.Eq(x => x.Email, input)
                    : builder.Eq(x => x.Mobile, input);

                var teacherFilter = builder.And(
                    loginFilter,
                    builder.Eq(x => x.Password, model.Password),
                    builder.Eq(x => x.Role, model.Role)
                );

                var teacher = await collection.Find(teacherFilter).FirstOrDefaultAsync();

                if (teacher == null)
                    return null;

                return new
                {
                    Name = teacher.TeacherName,
                    TeacherID= teacher.TeacherId,
                    Gender=teacher.Gender,
                    Qualification = teacher.Qualification,
                    Experience=teacher.Experience,
                    Email = teacher.Email,
                    Mobile = teacher.Mobile,
                    Role = teacher.Role,
                    Address = teacher.Address,
                    ImagePath=teacher.ImagePath,
                        Classes=teacher.Classes,
                        Subjects=teacher.Subjects,
                        AssignedClass=teacher.AssignedClass
                };
            }

            // ================= ADMIN LOGIN =================
            else
            {
                var collection = database.GetCollection<AdminCls>("LoginTB");

                var builder = Builders<AdminCls>.Filter;

                var loginFilter = isEmail
                    ? builder.Eq(x => x.Email, input)
                    : builder.Eq(x => x.Mobile, input);

                var adminFilter = builder.And(
                    loginFilter,
                    builder.Eq(x => x.Password, model.Password),
                    builder.Eq(x => x.Role, model.Role)
                );

                var admin = await collection.Find(adminFilter).FirstOrDefaultAsync();

                if (admin == null)
                    return null;

                return new
                {
                    Name = admin.AdminName,
                    AdminId=admin.AdminId,
                    Email = admin.Email,
                    Role = admin.Role,
                    Mobile = admin.Mobile,
                    Address=admin.Address,
                    ImagePath=admin.ImagePath,
                    cDt = admin.cDt,
                    uDt=admin.uDt,

                };
            }
        }





        public async Task<bool> Register(User model)
        {
            var client = new MongoClient(
                MongoClientSettings.FromConnectionString(
                    Connection.getAttendanceConnection()));

            var database = client.GetDatabase("DHs_4001");
            var collection = database.GetCollection<User>("LoginTB");

            var existsFilter = Builders<User>.Filter.Or(
                Builders<User>.Filter.Eq(x => x.Email, model.Email),
                Builders<User>.Filter.Eq(x => x.Mobile, model.Mobile)
            );

            var exists = await collection.Find(existsFilter).AnyAsync();

            if (exists)
                return false;

            await collection.InsertOneAsync(model);
            return true;
        }







        public async Task AddTeachers(List<TeacherCreateDto> teachers)
        {
            var mongoSettings = MongoClientSettings
                .FromConnectionString(Connection.getAttendanceConnection());

            mongoSettings.ServerSelectionTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.SocketTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.ConnectTimeout = TimeSpan.FromMinutes(5);

            var client = new MongoClient(mongoSettings);
            var database = client.GetDatabase("DHs_4001");
            var collection = database.GetCollection<TeacherCreateDto>("TeacherTB");

            foreach (var t in teachers)
            {
                
                t.TeacherId = await GetNextTeacherId(database);
                t.EmployeeId = await GetNextEmployeeId(database);
                t.cDt = DateTime.UtcNow;
                t.Role = "teacher";
           
                // =========================
                // Base64 Image Handling
                // =========================
                if (!string.IsNullOrWhiteSpace(t.ImageBase64))
                {
                    t.ImagePath = SaveBase64Image(t.ImageBase64);

                    // ❌ Never store base64 in Mongo
                    t.ImageBase64 = null;
                }
            }

            var options = new InsertManyOptions { IsOrdered = false };
            await collection.InsertManyAsync(teachers, options);
        }

        public async Task<List<TeacherCreateDto>> GetAllTeacher(TeacherCreateDto input)
        {
            var mongoSettings = MongoClientSettings
                  .FromConnectionString(Connection.getAttendanceConnection());

            mongoSettings.ServerSelectionTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.SocketTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.ConnectTimeout = TimeSpan.FromMinutes(5);

            var client = new MongoClient(mongoSettings);
            var database = client.GetDatabase("DHs_4001");

            var collection = database.GetCollection<TeacherCreateDto>("TeacherTB");

            var builder = Builders<TeacherCreateDto>.Filter;
            var filter = builder.Empty;
            if (!string.IsNullOrWhiteSpace(input.TeacherName))
            {
                filter = Builders<TeacherCreateDto>.Filter.Eq(b => b.TeacherName, input.TeacherName);
            }
            var getallData = collection.Find(filter).ToList();
            return getallData;
        }

        public async Task<List<StudentCls>> GetAllStudent(StudentCls input)
        {
            var mongoSettings = MongoClientSettings
                .FromConnectionString(Connection.getAttendanceConnection());

            mongoSettings.ServerSelectionTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.SocketTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.ConnectTimeout = TimeSpan.FromMinutes(5);

            var client = new MongoClient(mongoSettings);
            var database = client.GetDatabase("DHs_4001");

            var collection = database.GetCollection<StudentCls>("StudentTB");

            var builder = Builders<StudentCls>.Filter;
            var filter = builder.Empty;

           

            var data = await collection.Find(filter).ToListAsync();
            
            return data;
        }

        public async Task<List<StudentCls>> GetAllStudentClassWise(StudentCls input)
        {
            var mongoSettings = MongoClientSettings
                .FromConnectionString(Connection.getAttendanceConnection());

            mongoSettings.ServerSelectionTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.SocketTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.ConnectTimeout = TimeSpan.FromMinutes(5);

            var client = new MongoClient(mongoSettings);
            var database = client.GetDatabase("DHs_4001");

            var collection = database.GetCollection<StudentCls>("StudentTB");

            var builder = Builders<StudentCls>.Filter;
            var filter = builder.Empty;

            // ✅ Class filter
            if (!string.IsNullOrWhiteSpace(input.Classes))
            {
                filter = builder.Eq(x => x.Classes, input.Classes);
            }

            // ✅ Student Name filter (AND condition)
            if (!string.IsNullOrWhiteSpace(input.StudentName))
            {
                filter = filter & builder.Eq(x => x.StudentName, input.StudentName);
            }

            var data = await collection.Find(filter).ToListAsync();

            return data;
        }


        public async Task<(long StudentCount, long TeacherCount)> CountAllTeacherStudent(StudentCls input)
        {
            var mongoSettings = MongoClientSettings
                .FromConnectionString(Connection.getAttendanceConnection());

            mongoSettings.ServerSelectionTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.SocketTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.ConnectTimeout = TimeSpan.FromMinutes(5);

            var client = new MongoClient(mongoSettings);
            var database = client.GetDatabase("DHs_4001");

            var collectionTeacher = database.GetCollection<TeacherCreateDto>("TeacherTB");
            var collectionStudent = database.GetCollection<StudentCls>("StudentTB");

            var studentBuilder = Builders<StudentCls>.Filter;
            var studentFilter = studentBuilder.Empty;

            if (!string.IsNullOrWhiteSpace(input.StudentName))
            {
                studentFilter = studentBuilder.Eq(b => b.StudentName, input.StudentName);
            }

            var teacherBuilder = Builders<TeacherCreateDto>.Filter;
            var teacherFilter = teacherBuilder.Empty;

            if (!string.IsNullOrWhiteSpace(input.StudentName))
            {
                teacherFilter = teacherBuilder.Eq(t => t.TeacherName, input.StudentName);
            }

            var countStudent = await collectionStudent.CountDocumentsAsync(studentFilter);
            var countTeacher = await collectionTeacher.CountDocumentsAsync(teacherFilter);

            return (countStudent, countTeacher);
        }





        public string SaveBase64Image(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64))
                return null;

            // 🔥 Remove data:image/... prefix
            if (base64.Contains(","))
                base64 = base64.Substring(base64.IndexOf(",") + 1);

            base64 = base64.Trim();

            // 🔥 Validate Base64 safely
            if (!IsValidBase64(base64))
                throw new ArgumentException("Invalid base64 image format");

            byte[] imageBytes;

            try
            {
                imageBytes = Convert.FromBase64String(base64);
            }
            catch
            {
                throw new ArgumentException("Invalid base64 image data");
            }

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}.png";
            var filePath = Path.Combine(folderPath, fileName);

            File.WriteAllBytes(filePath, imageBytes);

            return $"/uploads/{fileName}";
        }

        private bool IsValidBase64(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out _);
        }



        // Auto-increment TeacherId using counters collection
        private async Task<string> GetNextTeacherId(IMongoDatabase db)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", "teacherId");
            var update = Builders<BsonDocument>.Update.Inc("seq", 1);
            var options = new FindOneAndUpdateOptions<BsonDocument>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            var counter = await db.GetCollection<BsonDocument>("counters")
                                  .FindOneAndUpdateAsync(filter, update, options);

            return "T" + counter["seq"].AsInt32.ToString("D5"); // T00001, T00002...
        }
        private async Task<string> GetNextStudentId(IMongoDatabase db)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", "studentid");
            var update = Builders<BsonDocument>.Update.Inc("seq", 1);
            var options = new FindOneAndUpdateOptions<BsonDocument>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            var counter = await db.GetCollection<BsonDocument>("counters")
                                  .FindOneAndUpdateAsync(filter, update, options);

            return "T" + counter["seq"].AsInt32.ToString("D5"); // T00001, T00002...
        }

        // Auto-increment EmployeeId using counters collection
        private async Task<int> GetNextEmployeeId(IMongoDatabase db)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", "employeeId"); // ✅ Different counter
            var update = Builders<BsonDocument>.Update.Inc("seq", 1);
            var options = new FindOneAndUpdateOptions<BsonDocument>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            var counter = await db.GetCollection<BsonDocument>("counters")
                                  .FindOneAndUpdateAsync(filter, update, options);

            return counter["seq"].AsInt32; // 1001, 1002...
        }



        //internal async Task<(bool EmailExists, bool MobileExists)> CheckEmailMobileExistsAsync(List<string> emails, List<string> mobiles)
        //{
        //    var mongoSettings = MongoClientSettings
        //        .FromConnectionString(Connection.getAttendanceConnection());

        //    mongoSettings.ServerSelectionTimeout = TimeSpan.FromMinutes(5);
        //    mongoSettings.SocketTimeout = TimeSpan.FromMinutes(5);
        //    mongoSettings.ConnectTimeout = TimeSpan.FromMinutes(5);

        //    var client = new MongoClient(mongoSettings);
        //    var database = client.GetDatabase("TeacherDB");

        //    // ✅ Use TeacherCreateDto, NOT BsonDocument
        //    var collection = database.GetCollection<TeacherCreateDto>("TeacherTB");
        //    var existing = await collection
        //        .Find(x => emails.Contains(x.Email) || mobiles.Contains(x.Mobile))
        //        .ToListAsync();

        //    return (
        //        existing.Any(x => emails.Contains(x.Email)),
        //        existing.Any(x => mobiles.Contains(x.Mobile))
        //    );
        //}
        public async Task<List<string>> GetExistingTeacherEmailsAsync(List<string> emails)
        {
            if (emails == null || !emails.Any())
                return new List<string>();

            var mongoSettings = MongoClientSettings
                .FromConnectionString(Connection.getAttendanceConnection());

            mongoSettings.ServerSelectionTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.SocketTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.ConnectTimeout = TimeSpan.FromMinutes(5);

            var client = new MongoClient(mongoSettings);
            var database = client.GetDatabase("DHs_4001");
            var collection = database.GetCollection<TeacherCreateDto>("TeacherTB");

            // ✅ Use In filter instead of Contains
            var filter = Builders<TeacherCreateDto>.Filter.In(x => x.Email, emails);

            var existingEmails = await collection
                .Find(filter)
                .Project(x => x.Email)   // ✅ Only fetch Email field (performance optimized)
                .ToListAsync();

            return existingEmails;
        }

        public async Task<List<string>> GetExistingAdminEmailsAsync(List<string> emails)
        {
            if (emails == null || !emails.Any())
                return new List<string>();

            var mongoSettings = MongoClientSettings
                .FromConnectionString(Connection.getAttendanceConnection());

            mongoSettings.ServerSelectionTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.SocketTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.ConnectTimeout = TimeSpan.FromMinutes(5);

            var client = new MongoClient(mongoSettings);
            var database = client.GetDatabase("DHs_4001");
            var collection = database.GetCollection<AdminCls>("LoginTB");

            // ✅ Use In filter instead of Contains
            var filter = Builders<AdminCls>.Filter.In(x => x.Email, emails);

            var existingEmails = await collection
                .Find(filter)
                .Project(x => x.Email)   // ✅ Only fetch Email field (performance optimized)
                .ToListAsync();

            return existingEmails;
        }

        public async Task<List<string>> GetExistingTeacherMobilesAsync(List<string> mobiles)
        {
            var mongoSettings = MongoClientSettings
                .FromConnectionString(Connection.getAttendanceConnection());

            mongoSettings.ServerSelectionTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.SocketTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.ConnectTimeout = TimeSpan.FromMinutes(5);

            var client = new MongoClient(mongoSettings);
            var database = client.GetDatabase("DHs_4001");

            var collection = database.GetCollection<TeacherCreateDto>("TeacherTB");

            // Find all teachers whose Email is in the provided list
            var existing = await collection
                .Find(x => mobiles.Contains(x.Mobile))
                .ToListAsync();

            // Return only existing emails
            return existing.Select(x => x.Mobile).ToList();
        }
        public async Task<List<string>> GetExistingStudentMobilesAsync(List<string> mobiles)
        {
            var mongoSettings = MongoClientSettings
                .FromConnectionString(Connection.getAttendanceConnection());

            mongoSettings.ServerSelectionTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.SocketTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.ConnectTimeout = TimeSpan.FromMinutes(5);

            var client = new MongoClient(mongoSettings);
            var database = client.GetDatabase("DHs_4001");

            var collection = database.GetCollection<StudentCls>("StudentTB");

            // Find all teachers whose Email is in the provided list
            var existing = await collection.Find(x => mobiles.Contains(x.Mobile)).ToListAsync();

            // Return only existing emails
            return existing.Select(x => x.Mobile).ToList();
        }

        public async Task<List<string>> GetExistingAdminMobilesAsync(List<string> mobiles)
        {
            var mongoSettings = MongoClientSettings
                .FromConnectionString(Connection.getAttendanceConnection());

            mongoSettings.ServerSelectionTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.SocketTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.ConnectTimeout = TimeSpan.FromMinutes(5);

            var client = new MongoClient(mongoSettings);
            var database = client.GetDatabase("DHs_4001");

            var collection = database.GetCollection<AdminCls>("LoginTB");

            // Find all teachers whose Email is in the provided list
            var existing = await collection.Find(x => mobiles.Contains(x.Mobile)).ToListAsync();

            // Return only existing emails
            return existing.Select(x => x.Mobile).ToList();
        }

        public async Task AddStudents(List<StudentCls> students)
        {
            var mongoSettings = MongoClientSettings
                .FromConnectionString(Connection.getAttendanceConnection());

            mongoSettings.ServerSelectionTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.SocketTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.ConnectTimeout = TimeSpan.FromMinutes(5);

            var client = new MongoClient(mongoSettings);
            var database = client.GetDatabase("DHs_4001");
            var collection = database.GetCollection<StudentCls>("StudentTB");

            foreach (var t in students)
            {

                t.StudentId = await GetNextStudentId(database);
              
                t.cDt = DateTime.UtcNow;
                t.Role = "student";

                // =========================
                // Base64 Image Handling
                // =========================
                if (!string.IsNullOrWhiteSpace(t.ImageBase64))
                {
                    t.ImagePath = SaveBase64Image(t.ImageBase64);

                    // ❌ Never store base64 in Mongo
                    t.ImageBase64 = null;
                }
            }

            var options = new InsertManyOptions { IsOrdered = false };
            await collection.InsertManyAsync(students, options);
        }


        public async Task AddAdmin(List<AdminCls> admins)
        {
            var mongoSettings = MongoClientSettings
                .FromConnectionString(Connection.getAttendanceConnection());

            mongoSettings.ServerSelectionTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.SocketTimeout = TimeSpan.FromMinutes(5);
            mongoSettings.ConnectTimeout = TimeSpan.FromMinutes(5);

            var client = new MongoClient(mongoSettings);
            var database = client.GetDatabase("DHs_4001");
            var collection = database.GetCollection<AdminCls>("LoginTB");

            foreach (var t in admins)
            {

                t.AdminId = await GetNextStudentId(database);

                t.cDt = DateTime.UtcNow;
                t.Role = "Admin";

                // =========================
                // Base64 Image Handling
                // =========================
                if (!string.IsNullOrWhiteSpace(t.ImageBase64))
                {
                    t.ImagePath = SaveBase64Image(t.ImageBase64);

                    // ❌ Never store base64 in Mongo
                    t.ImageBase64 = null;
                }
            }

            var options = new InsertManyOptions { IsOrdered = false };
            await collection.InsertManyAsync(admins, options);
        }

        public async Task<ApiResponse> MarkAttendancePunchIn(AttendanceCls model)
        {
            try
            {
                var client = new MongoClient(Connection.getAttendanceConnection());
                var database = client.GetDatabase("DHs_4001");
                var collection = database.GetCollection<AttendanceCls>("AttendeceTB");

                DateTime utcTime = DateTime.UtcNow;
                DateTime istTime = utcTime.AddHours(5.5);

                DateTime startDate = istTime.Date;
                DateTime endDate = startDate.AddDays(1);

                // ✅ Filter (StudentId + Today)
                var filter = Builders<AttendanceCls>.Filter.And(
                    Builders<AttendanceCls>.Filter.Eq(x => x.StudentId, model.StudentId),
                    Builders<AttendanceCls>.Filter.Gte(x => x.PunchInTime, startDate),
                    Builders<AttendanceCls>.Filter.Lt(x => x.PunchInTime, endDate)
                );

                var existing = await collection.Find(filter).FirstOrDefaultAsync();

                // ✅ Already Punch-In
                if (existing != null)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Already punch-in today"
                    };
                }

                // ✅ Insert
                var attendanceObj = new AttendanceCls
                {
                    StudentId = model.StudentId,
                    StudentName = model.StudentName,
                    Role = model.Role,
                    AttendanceDate = istTime,
                    Status = model.Status,
                    TeacherId = model.TeacherId,
                    PunchInTime = istTime,
                    CheckOutTime = null,
                    cDt = istTime
                };

                await collection.InsertOneAsync(attendanceObj);

                return new ApiResponse
                {
                    Success = true,
                    Message = "Punch-in successful",
                    Data = new
                    {
                        attendanceObj.StudentName,
                        attendanceObj.StudentId,
                        attendanceObj.AttendanceDate,
                        attendanceObj.Status,
                        attendanceObj.TeacherId,
                        attendanceObj.PunchInTime
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<string> MarkAttendancePunchOut(AttendanceCls model)
        {
            var client = new MongoClient(Connection.getAttendanceConnection());
            var database = client.GetDatabase("DHs_4001");
            var collection = database.GetCollection<AttendanceCls>("AttendeceTB");

            DateTime utcTime = DateTime.UtcNow;
            DateTime istTime = utcTime.AddHours(5.5);

            DateTime startDate = istTime.Date;
            DateTime endDate = startDate.AddDays(1);

            // ✅ Filter by StudentId + today's record
            var filter = Builders<AttendanceCls>.Filter.And(
                Builders<AttendanceCls>.Filter.Eq(x => x.StudentId, model.StudentId),
                Builders<AttendanceCls>.Filter.Gte(x => x.PunchInTime, startDate),
                Builders<AttendanceCls>.Filter.Lt(x => x.PunchInTime, endDate)
            );

            var existing = await collection.Find(filter).FirstOrDefaultAsync();

            if (existing == null)
            {
                return "No punch-in found for today";
            }

            // ✅ Check already punched out
            if (existing.CheckOutTime != null)
            {
                return "Already punch-out today";
            }

            // ✅ Update CheckOutTime
            var update = Builders<AttendanceCls>.Update
                .Set(x => x.CheckOutTime, istTime);

            await collection.UpdateOneAsync(filter, update);

            return "Punch-out successful";
        }
        public async Task<object> AttendenceCount(AttendanceCls model)
        {
            var client = new MongoClient(Connection.getAttendanceConnection());
            var database = client.GetDatabase("DHs_4001");
            var collection = database.GetCollection<AttendanceCls>("AttendeceTB");

            var result = await collection.Aggregate()
                .Group(x => 1, g => new
                {
                    TotalCount = g.Count(),
                    PresentCount = g.Sum(x => x.Status == "Present" ? 1 : 0),
                    AbsentCount = g.Sum(x => x.Status == "Absent" ? 1 : 0)
                })
                .FirstOrDefaultAsync();

            return result;
        }
    }
}
