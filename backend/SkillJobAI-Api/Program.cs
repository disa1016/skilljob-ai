using Microsoft.EntityFrameworkCore;
using SkillJobAI.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Controller aktivieren
builder.Services.AddControllers();

// PostgreSQL verbinden
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger aktivieren
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger nur in Development anzeigen
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS vorerst deaktiviert
// app.UseHttpsRedirection();

// Controller-Routen aktivieren
app.MapControllers();

app.Run();