using Microsoft.AspNetCore.Mvc;
using WebApiNew.Model;

namespace WebApiNew.Controllers
{
    [ApiController]
    [Route("api/Auth")]

    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("Login")]

   
        public async Task<IActionResult> Login([FromBody] TeacherCreateDto model)
        {
            // Remove unwanted validations
            ModelState.Remove("Gender");
            ModelState.Remove("Mobile");
            ModelState.Remove("Email");
            ModelState.Remove("Address");
            ModelState.Remove("Classes");
            ModelState.Remove("Subjects");
            ModelState.Remove("Experience");
            ModelState.Remove("TeacherName");
            ModelState.Remove("Qualification");
            ModelState.Remove("AssignedClass");

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Email/Mobile and Password are required",
                    errors = ModelState.Values
                                .SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage)
                });
            }

            var result = await _authService.Login(model);

            if (result == null)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid Email/Mobile or Password"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Login successful.",
                data = result
            });
        }




        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] User model)
        {
            //if (model == null || !ModelState.IsValid)
            //{
            //    return BadRequest(new
            //    {
            //        success = false,
            //        message = "All fields are required"
            //    });
            //}

            var result = await _authService.Register(model);

            if (result == false)
            {
                return Conflict(new   // 409
                {
                    success = false,
                    message = "Email or Mobile already exists"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Registration successful"
            });
        }


    }
}
