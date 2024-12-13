using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Mappings;

public class BooksMapping : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder
            .Property(e => e.Language)
            .HasConversion(
                v => v.ToString(),
                v => (Language)Enum.Parse(typeof(Language), v));

        builder
            .HasMany(e => e.Authors)
            .WithMany(e => e.Books);
    }
}