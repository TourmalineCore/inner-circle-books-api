using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Mappings;

public class BooksCopiesReadingHistoryMapping : IEntityTypeConfiguration<BookCopyReadingHistory>
{
    public void Configure(EntityTypeBuilder<BookCopyReadingHistory> builder)
    {
        builder
            .Property(e => e.ProgressOfReading)
            .HasConversion(
                v => v.ToString(),
                v => (ProgressOfReading)Enum.Parse(typeof(ProgressOfReading), v));

        builder
            .HasOne(e => e.BookCopy)
            .WithMany(e => e.ReadingHistoryList)
            .HasForeignKey(e => e.BookCopyId)
            .IsRequired();
    }
}
