using Application.Commands;
using Core;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Testcontainers.PostgreSql;
using Xunit;

namespace Application.Services
{
    public class TakeBookServiceTests : IAsyncLifetime
    {
        private readonly TakeBookService _service;
        private readonly AppDbContext _context;
        private readonly PostgreSqlContainer _postgreSqlContainer;
        private readonly Mock<IInnerCircleHttpClient> _clientMock;

        public TakeBookServiceTests()
        {
            _postgreSqlContainer = new PostgreSqlBuilder()
                .Build();

            _clientMock = new Mock<IInnerCircleHttpClient>();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options;

            _context = new AppDbContext(options);

            _service = new TakeBookService(_context, _clientMock.Object);

        }

        public async Task InitializeAsync()
        {
            await _postgreSqlContainer.StartAsync();
            await _context.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            await _postgreSqlContainer.DisposeAsync();
            await _context.DisposeAsync();
        }

        [Fact]
        public async Task TakeAsync_WhenBookIsAlreadyTaken_ShouldReturnPreviousAndAddNew()
        {
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
    }
}
