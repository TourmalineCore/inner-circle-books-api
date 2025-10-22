using Application.Commands;
using Core;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace Application.Services
{
    public class TakeBookServiceTests : IAsyncLifetime
    {
        private TakeBookService _service;
        private AppDbContext _context;
        private readonly PostgreSqlContainer _postgreSqlContainer;

        public TakeBookServiceTests()
        {
            _postgreSqlContainer = new PostgreSqlBuilder()
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _postgreSqlContainer.StartAsync();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options;

            _context = new AppDbContext(options);

            _service = new TakeBookService(_context);

            await _context.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            await _postgreSqlContainer.DisposeAsync();
            await _context.DisposeAsync();
        }

        // Todo: move to karate test?
        [Fact]
        public async Task TakeAsync_WhenBookIsAlreadyTaken_ShouldReturnPreviousAndAddNew()
        {
            var book = new Book
            {
                Id = 1,
                Title = "Some test book",
                Annotation = "Test annotation",
                TenantId = 1,
                CreatedAtUtc = DateTime.UtcNow,
                Language = Language.en,
                Authors = new List<Author>()
            };

            _context.Books.Add(book);

            await _context.SaveChangesAsync();

            var bookCopy = new BookCopy { Id = 2, BookId = book.Id };

            _context.BooksCopies.Add(bookCopy);

            await _context.SaveChangesAsync();

            var existingReader = new Employee { Id = 2, FullName = "Previous Reader" };

            _context.BooksCopiesReadingHistory.Add(new BookCopyReadingHistory
            {
                BookCopyId = 2,
                ReaderEmployeeId = existingReader.Id,
                TakenAtUtc = DateTime.UtcNow.AddDays(-10),
                ScheduledReturnDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                ActualReturnedAtUtc = null
            });

            await _context.SaveChangesAsync();

            var newEmployee = new Employee { Id = 3, FullName = "New Reader" };

            var takeBookCommandParams = new TakeBookCommandParams
            {
                BookCopyId = 2,
                ScheduledReturnDate = "2025-12-10"
            };

            var returnBookCommandParams = new ReturnBookCommandParams
            {
                BookCopyId = 2,
                ProgressOfReading = ProgressOfReading.NotSpecified,
                ActualReturnedAtUtc = DateTime.UtcNow
            };

            await _service.TakeAsync(takeBookCommandParams, returnBookCommandParams, newEmployee);

            var oldRecord = await _context.BooksCopiesReadingHistory
                .FirstAsync(x => x.ReaderEmployeeId == existingReader.Id && x.BookCopyId == 2);

            Assert.NotNull(oldRecord.ActualReturnedAtUtc);

            var newRecord = await _context.BooksCopiesReadingHistory
                .FirstOrDefaultAsync(x => x.ReaderEmployeeId == newEmployee.Id && x.BookCopyId == 2);

            Assert.NotNull(newRecord);
            Assert.Equal(new DateOnly(2025, 12, 10), newRecord.ScheduledReturnDate);
            Assert.Null(newRecord.ActualReturnedAtUtc);
        }

        [Fact]
        public async Task TakeAsync_WhenTakeCommandThrowsException_ShouldRollbackAndCallReturn()
        {
            var book = new Book
            {
                Id = 1,
                Title = "Some test book",
                Annotation = "Test annotation",
                TenantId = 1,
                CreatedAtUtc = DateTime.UtcNow,
                Language = Language.en,
                Authors = new List<Author>()
            };

            _context.Books.Add(book);

            await _context.SaveChangesAsync();

            var bookCopy = new BookCopy { Id = 2, BookId = book.Id };

            _context.BooksCopies.Add(bookCopy);

            await _context.SaveChangesAsync();

            var existingReader = new Employee { Id = 2, FullName = "Previous Reader" };

            _context.BooksCopiesReadingHistory.Add(new BookCopyReadingHistory
            {
                BookCopyId = 2,
                ReaderEmployeeId = existingReader.Id,
                TakenAtUtc = DateTime.UtcNow.AddDays(-10),
                ScheduledReturnDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                ActualReturnedAtUtc = null
            });

            await _context.SaveChangesAsync();

            var newEmployee = new Employee { Id = 3, FullName = "New Reader" };

            var takeBookCommandParams = new TakeBookCommandParams
            {
                // Sending a non-existent BookCopyId so that takeBookCommand throws an exception and we can verify that the transaction has been rolled back.
                BookCopyId = 3,
                ScheduledReturnDate = "2025-12-10"
            };

            var returnBookCommandParams = new ReturnBookCommandParams
            {
                BookCopyId = 2,
                ProgressOfReading = ProgressOfReading.NotSpecified,
                ActualReturnedAtUtc = DateTime.UtcNow
            };

            await _service.TakeAsync(takeBookCommandParams, returnBookCommandParams, newEmployee);

            // Without this, EF returns data from the cache of tracked entities, rather than from the database.
            // This method clears it, after which fresh data is returned from the database.
            _context.ChangeTracker.Clear();

            var oldRecord = await _context.BooksCopiesReadingHistory
                .FirstAsync(x => x.ReaderEmployeeId == existingReader.Id && x.BookCopyId == 2);

            Assert.Null(oldRecord.ActualReturnedAtUtc);

            var newRecord = await _context.BooksCopiesReadingHistory
                .FirstOrDefaultAsync(x => x.ReaderEmployeeId == newEmployee.Id && x.BookCopyId == 3);

            Assert.Null(newRecord);
        }
    }
}
