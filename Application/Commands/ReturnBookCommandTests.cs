// using Application;
// using Application.Commands;
// using Core.Entities;
// using Microsoft.EntityFrameworkCore;
// using Xunit;

// public class ReturnBookCommandTests
// {
//     private readonly ReturnBookCommand _command;
//     private readonly AppDbContext _context;

//     public ReturnBookCommandTests()
//     {
//         var options = new DbContextOptionsBuilder<AppDbContext>()
//             .UseInMemoryDatabase("ReturnBookCommandBooksDatabase")
//             .Options;

//         _context = new AppDbContext(options);
//         _command = new ReturnBookCommand(_context);
//     }

//     [Fact]
//     public async Task ReturnAsync_ShouldSetDifferentValues()
//     {
//         var dateTimeUtcPlusTwoMonths = DateTime.UtcNow.AddMonths(2);

//         var bookCopyReadingHistory = new BookCopyReadingHistory
//         {
//             Id = 1,
//             BookCopyId = 1,
//             ReaderEmployeeId = 1,
//             TakenAtUtc = DateTime.UtcNow,
//             ScheduledReturnDate = DateOnly.FromDateTime(dateTimeUtcPlusTwoMonths)
//         };

//         var returnBookRequest = new ReturnBookCommandParams
//         {
//             BookCopyId = 1,
//             ProgressOfReading = "Not Started",
//             ActualReturnedAtUtc = DateTime.UtcNow
//         };
//         _context.BooksCopiesReadingHistory.Add(bookCopyReadingHistory);
//         await _context.SaveChangesAsync();

//         await _command.ReturnAsync(returnBookRequest, 1);

//         var completedCopyReadingHistory = await _context.BooksCopiesReadingHistory.FindAsync(bookCopyReadingHistory.Id);
//         Assert.Equal(completedCopyReadingHistory.ActualReturnedAtUtc, returnBookRequest.ActualReturnedAtUtc);
//     }
// }