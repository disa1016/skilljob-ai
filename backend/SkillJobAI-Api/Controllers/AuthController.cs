using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillJobAI.Api.Data;
using SkillJobAI.Api.Entities;
using SkillJobAI.Api.Models;

namespace SkillJobAI.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email);

        if (emailExists)
        {
            return BadRequest(new
            {
                message = "Diese E-Mail ist bereits registriert."
            });
        }

        var user = new AppUser
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = request.Password,
            Role = "Student",
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "User registered successfully",
            user = new
            {
                id = user.Id,
                fullName = user.FullName,
                email = user.Email,
                role = user.Role
            }
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u =>
                u.Email == request.Email &&
                u.PasswordHash == request.Password);

        if (user == null)
        {
            return Unauthorized(new
            {
                message = "E-Mail oder Passwort ist falsch."
            });
        }

        return Ok(new
        {
            message = "Login successful",
            token = "fake-jwt-token-for-testing",
            user = new
            {
                id = user.Id,
                fullName = user.FullName,
                email = user.Email,
                role = user.Role
            }
        });
    }
}