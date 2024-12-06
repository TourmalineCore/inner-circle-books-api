using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Mappings
{
    public class BooksMapping : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasMany<Author>();
            builder.HasMany<Tag>();
            builder.HasMany<Category>();
            builder.HasOne<Language>();
            builder.HasOne<Status>();
        }
    }
}
