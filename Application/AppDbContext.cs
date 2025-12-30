using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
  {
  }

  public AppDbContext()
  {
  }

  public virtual DbSet<Book> Books { get; set; }

  public virtual DbSet<BookCopy> BooksCopies { get; set; }

  public virtual DbSet<BookCopyReadingHistory> BooksCopiesReadingHistory { get; set; }

  public virtual DbSet<KnowledgeArea> KnowledgeAreas { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<KnowledgeArea>().HasData(
        new KnowledgeArea { Id = 1, Name = "Frontend" },
        new KnowledgeArea { Id = 2, Name = "Backend" },
        new KnowledgeArea { Id = 3, Name = "ML" },
        new KnowledgeArea { Id = 4, Name = "DevOps" },
        new KnowledgeArea { Id = 5, Name = "QA" },
        new KnowledgeArea { Id = 6, Name = "Design" },
        new KnowledgeArea { Id = 7, Name = "Business and Management" },
        new KnowledgeArea { Id = 8, Name = "Embedded" },
        new KnowledgeArea { Id = 9, Name = "GameDev" },
        new KnowledgeArea { Id = 10, Name = "Marketing" },
        new KnowledgeArea { Id = 11, Name = "Information Security" },
        new KnowledgeArea { Id = 12, Name = "Psychology" },
        new KnowledgeArea { Id = 13, Name = "Copywriting and Editing" },
        new KnowledgeArea { Id = 14, Name = "Languages" }
    );
  }
}
