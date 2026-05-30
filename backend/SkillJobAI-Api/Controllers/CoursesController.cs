using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillJobAI.Api.Data;
using SkillJobAI.Api.Entities;

namespace SkillJobAI.Api.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CoursesController(AppDbContext context)
    {
        _context = context;
    }

    // Alle Kurse abrufen
    [HttpGet]
    public async Task<IActionResult> GetCourses()
    {
        var courses = await _context.Courses.ToListAsync();
        return Ok(courses);
    }

    // Einzelnen Kurs abrufen
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourse(int id)
    {
        var course = await _context.Courses.FindAsync(id);

        if (course == null)
            return NotFound();

        return Ok(course);
    }

    // Kurs erstellen - nur eingeloggte Benutzer
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateCourse(Course course)
    {
        course.CreatedAt = DateTime.UtcNow;

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        return Ok(course);
    }

    // Kurs löschen - nur eingeloggte Benutzer
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        var course = await _context.Courses.FindAsync(id);

        if (course == null)
            return NotFound();

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Course deleted successfully"
        });
    }
}