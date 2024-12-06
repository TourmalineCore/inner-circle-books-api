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

    public virtual DbSet<Book> Books { get; set; }
    public virtual DbSet<Author> Authors { get; set; }
    public virtual DbSet<Status> Statuses { get; set; }
    public virtual DbSet<Language> Languages { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        base.OnModelCreating(modelBuilder);
    }
}