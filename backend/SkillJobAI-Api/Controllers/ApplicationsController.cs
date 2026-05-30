using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillJobAI.Api.Data;
using SkillJobAI.Api.Entities;

namespace SkillJobAI.Api.Controllers;

[ApiController]
[Route("api/applications")]
public class ApplicationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ApplicationsController(AppDbContext context)
    {
        _context = context;
    }

    // Bewerbung erstellen
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateApplication(Application application)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        application.UserId = int.Parse(userId);
        application.CreatedAt = DateTime.UtcNow;
        application.Status = "Pending";

        _context.Applications.Add(application);
        await _context.SaveChangesAsync();

        return Ok(application);
    }

    // Meine Bewerbungen
    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> MyApplications()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        var applications = await _context.Applications
            .Where(a => a.UserId == int.Parse(userId))
            .ToListAsync();

        return Ok(applications);
    }
}