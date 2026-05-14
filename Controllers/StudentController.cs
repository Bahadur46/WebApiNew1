using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/student")]
[Authorize(Roles = "Admin,Student")]
public class StudentController : ControllerBase
{
    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        return Ok("Welcome Student ");
    }
}
