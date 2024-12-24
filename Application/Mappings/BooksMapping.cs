using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

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
            .Property(e => e.Authors)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<Author>>(v));
    }
}