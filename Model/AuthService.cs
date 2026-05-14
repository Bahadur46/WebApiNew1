

using WebApiNew;
using WebApiNew.Model;
using WebApiNew.Repository;

public class AuthService
{
    private readonly UserRepository _userRepo;

    public AuthService(UserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    // Add multiple teachers
    public async Task AddTeachers(List<TeacherCreateDto> teachers)
    {
        if (teachers == null || teachers.Count == 0)
            throw new ArgumentException("Teachers list is empty");

        await _userRepo.AddTeachers(teachers);
    }
    public async Task AddStudent(List<StudentCls> students)
    {
        if (students == null || students.Count == 0)
            throw new ArgumentException("Student list is empty");

        await _userRepo.AddStudents(students);
    }
    public async Task AddAdmin(List<AdminCls> admin)
    {
        if (admin == null || admin.Count == 0)
            throw new ArgumentException("Teachers list is empty");

        await _userRepo.AddAdmin(admin);
    }

    public async Task<List<string>> GetExistingTeacherEmailsAsync(List<string> emails)
    {
        return await _userRepo.GetExistingTeacherEmailsAsync(emails);
    }

    public async Task<List<string>> GetExistingAdminEmailsAsync(List<string> emails)
    {
        return await _userRepo.GetExistingAdminEmailsAsync(emails);
    }
    public async Task<List<string>> GetExistingTeacherMobilesAsync(List<string> mobiles)
    {
        return await _userRepo.GetExistingTeacherMobilesAsync(mobiles);
    }
    public async Task<List<string>> GetExistingAdminMobilesAsync(List<string> mobiles)
    {
        return await _userRepo.GetExistingAdminMobilesAsync(mobiles);
    }
    
    public async Task<object> Login(TeacherCreateDto model)
    {
        return await _userRepo.Login(model);
    }
    public async Task<bool> Register(User model)
    {
        return await _userRepo.Register(model);
    }


    public async Task<List<TeacherCreateDto>> GetAllTeacher(TeacherCreateDto input)
    {
        return await _userRepo.GetAllTeacher(input);
    }
    public async Task<List<StudentCls>> GetAllStudent(StudentCls input)
    {
        return await _userRepo.GetAllStudent(input);
    }
    public async Task<List<StudentCls>> GetAllStudentClassWise(StudentCls input)
    {
        return await _userRepo.GetAllStudentClassWise(input);
    }
    public Task<(long StudentCount, long TeacherCount)> CountAllTeacherStudent(StudentCls input)
    {
        return _userRepo.CountAllTeacherStudent(input);
    }

    public async Task<ApiResponse> MarkAttendancePunchIn(AttendanceCls model)
    {
        return await _userRepo.MarkAttendancePunchIn(model);
    }
    public async Task<string> MarkAttendancePunchOut(AttendanceCls model)
    {
        return await _userRepo.MarkAttendancePunchOut(model);
    }
    public async Task<object> AttendenceCount(AttendanceCls model)
    {
        return await _userRepo.AttendenceCount(model);
    }


    public async Task<List<string>> GetExistingStudentMobilesAsync(List<string> mobiles)
    {
        return await _userRepo.GetExistingStudentMobilesAsync(mobiles);
    }
}
