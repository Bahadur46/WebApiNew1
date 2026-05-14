using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiNew.Model;

[ApiController]
[Route("api/teacher")]
public class TeacherController : ControllerBase
{
    private readonly AuthService _authService;

    public TeacherController(AuthService authService)
    {
        _authService = authService;
    }

    //// ✅ Teacher Signup
    //[HttpPost("register")]
    //[AllowAnonymous]
    //public async Task<IActionResult> RegisterTeacher([FromBody] User user)
    //{
    //    var result = await _authService.RegisterTeacher(user);
    //    return Ok(result);
    //}

    //// ✅ Teacher Login
    //[HttpPost("login")]
    //[AllowAnonymous]
    //public async Task<IActionResult> LoginTeacher([FromBody] LoginRequest model)
    //{
    //    var token = await _authService.LoginTeacher(model);
    //    if (token == null)
    //        return Unauthorized("Invalid credentials");

    //    return Ok(new { Token = token, Role = "Teacher" });
    //}

}