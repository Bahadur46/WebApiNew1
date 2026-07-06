using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiNew;
using WebApiNew.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly AuthService _authService;

    public AdminController(AuthService authService)
    {
        _authService = authService;
    }


    //[HttpPost("add-teacher")]
    //public async Task<IActionResult> AddTeacher([FromBody] TeacherCreateDto model)
    //{
    //    //  Proper null check
    //    if (model == null)
    //    {
    //        return BadRequest(new
    //        {
    //            success = false,
    //            message = "Request body is required"
    //        });
    //    }

    //    //  Manual required fields check (recommended)
    //    if (string.IsNullOrWhiteSpace(model.TeacherName)
    //        || string.IsNullOrWhiteSpace(model.Email)
    //        || string.IsNullOrWhiteSpace(model.Mobile) ||
    //            string.IsNullOrWhiteSpace(model.Gender)
    //         || string.IsNullOrWhiteSpace(model.Address)
    //          || string.IsNullOrWhiteSpace(model.Qualification)
    //           || string.IsNullOrWhiteSpace(model.Experience)
    //        || string.IsNullOrWhiteSpace(model.Password))
    //    {
    //        return BadRequest(new
    //        {
    //            success = false,
    //            message = "All required fields must be filled"
    //        });
    //    }

    //    await _authService.AddTeachers(new List<TeacherCreateDto> { model });

    //    return Ok(new
    //    {
    //        success = true,
    //        message = "Teacher registered successfully"
    //    });
    //}
    [HttpPost("add-teacher")]
    public async Task<IActionResult> AddTeacher([FromBody] TeacherCreateDto model)
    {
        //if (model == null || !ModelState.IsValid)
        //{
        //    return BadRequest(new
        //    {
        //        success = false,
        //        message = "All fields are required"
        //    });
        //}
        var existingEmails = await _authService
            .GetExistingTeacherEmailsAsync(new List<string> { model.Email });

        var existingMobiles = await _authService
            .GetExistingTeacherMobilesAsync(new List<string> { model.Mobile });

        if (existingEmails.Any() || existingMobiles.Any())
        {
            return Ok(new
            {
                success = false,
                message = "Teacher already registered"
            });
        }

        try
        {

            if (!string.IsNullOrWhiteSpace(model.ImageBase64))
            {

                string cleanBase64 = model.ImageBase64.Contains(",")
                    ? model.ImageBase64.Split(',')[1]
                    : model.ImageBase64;


                string? imageUrl = await ImageUploadHelper.UploadToImgBB(cleanBase64);

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    model.ImagePath = imageUrl; 
                }

                model.ImageBase64 = null; 
            }

           
            await _authService.AddTeachers(new List<TeacherCreateDto> { model });

            return Ok(new
            {
                success = true,
                message = "Teacher registered successfully",
                data = new
                {
                    user_id = model.TeacherId,
                    name = model.TeacherName,
                    email = model.Email,
                    mobile = model.Mobile,

                }
            });
        }
        catch (Exception ex)
        {
            return Ok(new
            {
                success = false,
                message = "Registration failed. Please try again",
                
            });
        }
    }

    [HttpPost("add-student")]
    public async Task<IActionResult> AddStudent([FromBody] StudentCls model)
    {
        ModelState.Remove("Role");
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                message = "All fields are required",
                errors = ModelState
            });
        }


        var existingMobiles = await _authService
            .GetExistingStudentMobilesAsync(new List<string> { model.Mobile });

        if (existingMobiles.Any())
        {
            return Ok(new
            {
                success = false,
                message = "User already registered"
            });
        }

        try
        {
            
            if (!string.IsNullOrWhiteSpace(model.ImageBase64))
            {
                string cleanBase64 = model.ImageBase64.Contains(",")
                    ? model.ImageBase64.Split(',')[1]
                    : model.ImageBase64;



                string? imageUrl = await ImageUploadHelper.UploadToImgBB(cleanBase64);


                if (!string.IsNullOrEmpty(imageUrl))
                {
                    model.ImagePath = imageUrl;
                }

                model.ImageBase64 = null; 
            }

            // Save to DB
            await _authService.AddStudent(new List<StudentCls> { model });

            return Ok(new
            {
                success = true,
                message = "Student registered successfully",
                data = new
                {
                    Student_Id = model.StudentId,
                    Student_Name = model.StudentName,
                    Mobile = model.Mobile,
                    
                }
            });
        }
        catch (Exception ex)
        {
            return Ok(new
            {
                success = false,
                message = "Registration failed",
                error = ex.Message
            });
        }
    }
    [HttpPost("add-admin")]
    public async Task<IActionResult> AddAdmin([FromBody] AdminCls model)
    {
        if (model == null || !ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                message = "All fields are required"
            });
        }


        var existingEmails = await _authService
            .GetExistingAdminEmailsAsync(new List<string> { model.Email });

        var existingMobiles = await _authService
            .GetExistingAdminMobilesAsync(new List<string> { model.Mobile });

        if (existingEmails.Any() || existingMobiles.Any())
        {
            return Ok(new
            {
                success = false,
                message = "Admin already registered"
            });
        }

        try
        {

            if (!string.IsNullOrWhiteSpace(model.ImageBase64))
            {

                string cleanBase64 = model.ImageBase64.Contains(",")
                    ? model.ImageBase64.Split(',')[1]
                    : model.ImageBase64;


                string? imageUrl = await ImageUploadHelper.UploadToImgBB(cleanBase64);

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    model.ImagePath = imageUrl;
                }

                model.ImageBase64 = null;
            }


            await _authService.AddAdmin(new List<AdminCls> { model });

            return Ok(new
            {
                    success = true,
                    message = "Admin registered successfully",
                    data = new
                    {
                        user_id = model.AdminId,
                        name = model.AdminName,
                        email = model.Email,
                        mobile = model.Mobile,

                    }
            });
        }
        catch (Exception ex)
        {
            return Ok(new
            {
                success = false,
                message = "Registration failed. Please try again",

            });
        }
    }

    [HttpGet("GetAllTeacher")]
    public async Task<ActionResult<List<TeacherCreateDto>>> GetAllTeacher([FromQuery] TeacherCreateDto input)
    {
        try
        {
            var result = await _authService.GetAllTeacher(input);

            return Ok(new
            {
                success = true,
                message = "Success",
                data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = ex.Message
            });
        }



    }
    [HttpPost("GetAllStudentClassWise")]
    public async Task<ActionResult<List<StudentCls>>> GetAllStudentClassWise([FromBody] StudentCls input)
    {
        try
        {
            var result = await _authService.GetAllStudentClassWise(input);

            if (result != null && result.Count > 0)
            {
                return Ok(new
                {
                    success = true,
                    message = "Success",
                    data = result
                });
            }
            else
            {
                return Ok(new
                {
                    success = true,
                    message = "No records found",
                    data = result
                });
            }


        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = ex.Message
            });
        }
    }


    [HttpGet("GetAllStudent")]
    public async Task<ActionResult<List<StudentCls>>> GetAllStudent([FromQuery] StudentCls input)
    {
        try
        {
            var result = await _authService.GetAllStudent(input);

            if (result != null && result.Count > 0)
            {
                return Ok(new
                {
                    success = true,
                    message = "Success",
                    data = result
                });
            }
            else
            {
                return Ok(new
                {
                    success = true,
                    message = "No records found",
                    data = result
                });
            }


        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = ex.Message
            });
        }
    }


    [HttpGet("CountAllTeacherStudent")]
            public async Task<IActionResult> CountAllTeacherStudent([FromQuery] StudentCls input)
            {
                var result = await _authService.CountAllTeacherStudent(input);

                return Ok(new
                {
                    success = true,
                    message = "Count fetched successfully",
                    data = new
                    {
                        TotalStudents = result.StudentCount,
                        TotalTeachers = result.TeacherCount
                    }
                });
            }

        }
 


