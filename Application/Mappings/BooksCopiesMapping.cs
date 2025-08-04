using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Mappings;

public class BooksCopiesMapping : IEntityTypeConfiguration<BookCopy>
{
    public void Configure(EntityTypeBuilder<BookCopy> builder)
    {
        builder
            .HasOne(e => e.Book)
            .WithMany(e => e.Copies)
            .HasForeignKey(e => e.BookId)
            .IsRequired();
    }
}
