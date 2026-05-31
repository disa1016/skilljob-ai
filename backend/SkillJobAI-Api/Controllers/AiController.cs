using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillJobAI.Api.Data;
using UglyToad.PdfPig;

namespace SkillJobAI.Api.Controllers;

[ApiController]
[Route("api/ai")]
public class AiController : ControllerBase
{
    private readonly AppDbContext _context;

    public AiController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpPost("analyze-cv")]
    public IActionResult AnalyzeCv(AnalyzeCvRequest request)
    {
        return Ok(AnalyzeCvText(request.CvText));
    }

    [Authorize]
    [HttpPost("analyze-cv-pdf")]
    public async Task<IActionResult> AnalyzeCvPdf(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new
            {
                message = "Bitte lade eine PDF-Datei hoch."
            });
        }

        if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                message = "Nur PDF-Dateien sind erlaubt."
            });
        }

        var cvText = new StringBuilder();

        await using var stream = file.OpenReadStream();

        using var document = PdfDocument.Open(stream);

        foreach (var page in document.GetPages())
        {
            cvText.AppendLine(page.Text);
        }

        if (string.IsNullOrWhiteSpace(cvText.ToString()))
        {
            return BadRequest(new
            {
                message = "Aus dieser PDF konnte kein Text gelesen werden."
            });
        }

        var result = AnalyzeCvText(cvText.ToString());

        return Ok(new
        {
            fileName = file.FileName,
            extractedText = cvText.ToString(),
            result.Score,
            result.Skills,
            result.Suggestions
        });
    }

    [Authorize]
    [HttpPost("job-match")]
    public IActionResult JobMatch(JobMatchRequest request)
    {
        var result = CalculateJobMatch(
            request.CvText,
            request.JobDescription
        );

        return Ok(new
        {
            matchScore = result.MatchScore,
            matchedSkills = result.MatchedSkills,
            missingSkills = result.MissingSkills,
            recommendation = GetRecommendation(result.MatchScore)
        });
    }

    [Authorize]
    [HttpPost("job-recommendations")]
    public async Task<IActionResult> JobRecommendations(JobRecommendationsRequest request)
    {
        var jobs = await _context.Jobs.ToListAsync();

        var recommendations = jobs
            .Select(job =>
            {
                var jobDescription =
                    $"{job.Title} {job.Description} {job.Company} {job.Location}";

                var result = CalculateJobMatch(
                    request.CvText,
                    jobDescription
                );

                return new
                {
                    jobId = job.Id,
                    title = job.Title,
                    company = job.Company,
                    location = job.Location,
                    salary = job.Salary,
                    description = job.Description,
                    matchScore = result.MatchScore,
                    matchedSkills = result.MatchedSkills,
                    missingSkills = result.MissingSkills,
                    recommendation = GetRecommendation(result.MatchScore)
                };
            })
            .OrderByDescending(r => r.matchScore)
            .ToList();

        return Ok(recommendations);
    }

    private static AnalyzeCvResult AnalyzeCvText(string cvText)
    {
        var text = cvText.ToLower();

        var skills = new List<string>();

        if (text.Contains("c#") || text.Contains("csharp"))
            skills.Add("C#");

        if (text.Contains("asp.net") || text.Contains(".net"))
            skills.Add("ASP.NET Core");

        if (text.Contains("vue"))
            skills.Add("Vue.js");

        if (text.Contains("javascript"))
            skills.Add("JavaScript");

        if (text.Contains("sql") || text.Contains("postgresql"))
            skills.Add("SQL / PostgreSQL");

        if (text.Contains("git") || text.Contains("github"))
            skills.Add("Git / GitHub");

        var score = Math.Min(100, skills.Count * 15);

        var suggestions = new List<string>();

        if (!skills.Contains("C#"))
            suggestions.Add("Füge C# Kenntnisse hinzu.");

        if (!skills.Contains("ASP.NET Core"))
            suggestions.Add("Erwähne ASP.NET Core oder Backend-Erfahrung.");

        if (!skills.Contains("Vue.js"))
            suggestions.Add("Erwähne Vue.js oder Frontend-Projekte.");

        if (!skills.Contains("Git / GitHub"))
            suggestions.Add("Erwähne GitHub-Projekte oder Versionskontrolle.");

        if (suggestions.Count == 0)
            suggestions.Add("Dein Lebenslauf enthält bereits gute technische Skills.");

        return new AnalyzeCvResult
        {
            Score = score,
            Skills = skills,
            Suggestions = suggestions
        };
    }

    private static JobMatchResult CalculateJobMatch(
        string cvTextInput,
        string jobTextInput)
    {
        var cvText = cvTextInput.ToLower();
        var jobText = jobTextInput.ToLower();

        var skills = new List<string>
        {
            "c#",
            "asp.net",
            ".net",
            "vue",
            "javascript",
            "sql",
            "postgresql",
            "git",
            "github",
            "docker",
            "azure"
        };

        var matchedSkills = new List<string>();
        var missingSkills = new List<string>();

        foreach (var skill in skills)
        {
            var skillInJob = jobText.Contains(skill);
            var skillInCv = cvText.Contains(skill);

            if (skillInJob && skillInCv)
                matchedSkills.Add(skill);

            if (skillInJob && !skillInCv)
                missingSkills.Add(skill);
        }

        var totalRequiredSkills =
            matchedSkills.Count + missingSkills.Count;

        var matchScore = totalRequiredSkills == 0
            ? 0
            : (int)Math.Round(
                (double)matchedSkills.Count /
                totalRequiredSkills * 100);

        return new JobMatchResult
        {
            MatchScore = matchScore,
            MatchedSkills = matchedSkills,
            MissingSkills = missingSkills
        };
    }

    private static string GetRecommendation(int matchScore)
    {
        return matchScore >= 80
            ? "Sehr guter Match. Du kannst dich auf diese Stelle bewerben."
            : matchScore >= 50
                ? "Guter Anfang. Ergänze noch fehlende Skills."
                : "Der Match ist noch niedrig. Verbessere deinen Lebenslauf gezielt.";
    }
}

public class AnalyzeCvRequest
{
    public string CvText { get; set; } = "";
}

public class AnalyzeCvResult
{
    public int Score { get; set; }

    public List<string> Skills { get; set; } = new();

    public List<string> Suggestions { get; set; } = new();
}

public class JobMatchRequest
{
    public string CvText { get; set; } = "";
    public string JobDescription { get; set; } = "";
}

public class JobRecommendationsRequest
{
    public string CvText { get; set; } = "";
}

public class JobMatchResult
{
    public int MatchScore { get; set; }

    public List<string> MatchedSkills { get; set; } = new();

    public List<string> MissingSkills { get; set; } = new();
}