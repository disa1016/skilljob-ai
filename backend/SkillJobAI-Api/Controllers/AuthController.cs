using Microsoft.AspNetCore.Mvc;
using SkillJobAI.Api.Models;

namespace SkillJobAI.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    [HttpPost("register")]
    public IActionResult Register(RegisterRequest request)
    {
        return Ok(new
        {
            message = "User registered successfully",
            user = new
            {
                fullName = request.FullName,
                email = request.Email
            }
        });
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        return Ok(new
        {
            message = "Login successful",
            token = "fake-jwt-token-for-testing"
        });
    }
}