using Microsoft.EntityFrameworkCore;
using mvc_web_project.Models;

namespace mvc_web_project.Data;

public class FitnessDbContext : DbContext
{
    public FitnessDbContext(DbContextOptions<FitnessDbContext> options) : base(options)
    {

    }
    public DbSet<User> Users { get; set; } = null!;
}