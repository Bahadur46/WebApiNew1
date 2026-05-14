using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using WebApiNew.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApiNew.Controllers
{
    [ApiController]
    [Route("api/Attendence")]

    public class AttendenceController : Controller
    {
        private readonly AuthService _authService;

        public AttendenceController(AuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("MarkAttendancePunchIn")]
        public async Task<IActionResult> MarkAttendancePunchIn([FromBody] AttendanceCls model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid attendance data",
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

            var result = await _authService.MarkAttendancePunchIn(model);

            // ✅ Direct response handle
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("MarkAttendancePunchOut")]
        public async Task<IActionResult> MarkAttendancePunchOut([FromBody] AttendanceCls model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid attendance data",
                    errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                });
            }


            var result = await _authService.MarkAttendancePunchOut(model);

            if (result == "Student already punch-out today")
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Student already punch-out today"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Punch-out successful"
            });

        }




        [HttpGet("AttendenceCount")]
        public async Task<IActionResult> AttendenceCount([FromQuery] AttendanceCls model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid attendance data",
                    errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                });
            }


            var result = await _authService.AttendenceCount(model);



            return Ok(new
            {
                success = true,
                message = "Attendance count fetched successfully",
                data = new
                {
                    count = result
                }
            });

        }
    }
}