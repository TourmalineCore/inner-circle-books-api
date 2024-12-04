using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application;

//Use next command in Package Manager Console to update Dev env DB
//PM> $env:ASPNETCORE_ENVIRONMENT = 'IDE'; Update-Database
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public AppDbContext()
    {
    }

    public virtual DbSet<ToDo> ToDos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        modelBuilder.Entity<ToDo>()
            .Property(f => f.Id)
            .ValueGeneratedOnAdd();
        base.OnModelCreating(modelBuilder);
    }
}