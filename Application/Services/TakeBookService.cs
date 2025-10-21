using Application.Commands;
using Core;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class TakeBookService
    {
        private readonly AppDbContext _context;

        private readonly TakeBookCommand _takeBookCommand;
      
        private readonly ReturnBookCommand _returnBookCommand;

        private readonly IInnerCircleHttpClient _client;

        public TakeBookService(AppDbContext context, IInnerCircleHttpClient client)
        {
            _context = context;
            _takeBookCommand = new TakeBookCommand(_context);
            _returnBookCommand = new ReturnBookCommand(_context);
            _client = client;
        }

        public async Task TakeAsync(TakeBookCommandParams takeBookCommandParams, ReturnBookCommandParams returnBookCommandParams, Employee employee)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var currentReader = await _context.BooksCopiesReadingHistory
                          //.Where(x => x.TenantId == tenantId)
                         .Where(x => x.BookCopyId == takeBookCommandParams.BookCopyId
                                  && x.ActualReturnedAtUtc == null)
                         .FirstOrDefaultAsync();

                if (currentReader != null)
                {
                    var employeesByIds = await _client.GetEmployeesByIdsAsync(new List<long> { currentReader.ReaderEmployeeId });

                    var employeeReader = new Employee
                    {
                        Id = employeesByIds[0].EmployeeId,
                        FullName = employeesByIds[0].FullName,
                    };

                    await _returnBookCommand.ReturnAsync(returnBookCommandParams, employeeReader);
                }

                await _takeBookCommand.TakeAsync(takeBookCommandParams, employee);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
            }
        }
    }

}
