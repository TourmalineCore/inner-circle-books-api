using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application;

//Use next command in Package Manager Console to edit Dev env DB
//PM> $env:ASPNETCORE_ENVIRONMENT = 'MockForDevelopment'; Edit-Database
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public AppDbContext()
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        base.OnModelCreating(modelBuilder);
    }
}