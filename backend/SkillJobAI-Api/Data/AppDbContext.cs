using Microsoft.EntityFrameworkCore;
using SkillJobAI.Api.Entities;

namespace SkillJobAI.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> Users => Set<AppUser>();

   public DbSet<Job> Jobs => Set<Job>();
}