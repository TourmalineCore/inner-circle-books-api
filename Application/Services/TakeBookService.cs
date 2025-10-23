using Application.Commands;
using Application.Queries;
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

        public TakeBookService(AppDbContext context)
        {
            _context = context;
            _takeBookCommand = new TakeBookCommand(_context);
            _returnBookCommand = new ReturnBookCommand(_context);
        }

        public async Task TakeAsync(TakeBookCommandParams takeBookCommandParams, ReturnBookCommandParams returnBookCommandParams, Employee employee, BookCopyReadingHistory? activeReading)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (activeReading != null)
                {
                    var employeeReader = new Employee
                    {
                        Id = activeReading.ReaderEmployeeId,
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