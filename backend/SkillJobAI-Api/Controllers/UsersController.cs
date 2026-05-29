
using Microsoft.AspNetCore.Mvc;

namespace SkillJobAI.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    [HttpGet("profile")]
    public IActionResult Profile()
    {
        return Ok(new
        {
            fullName = "Test User",
            email = "test@example.com",
            role = "Student"
        });
    }
}