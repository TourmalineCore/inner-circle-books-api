using Api.Requests;
using Application.Commands;
using Application.Queries;
using Core;
using Core.Entities;

namespace Api.Controllers.Handlers;

public class ReturnBookHandler
{
  private readonly GetBookByCopyIdQuery _getBookByCopyIdQuery;
  private readonly ReturnBookCommand _returnBookCommand;
    
  public ReturnBookHandler(
    GetBookByCopyIdQuery getBookByCopyIdQuery,
    ReturnBookCommand returnBookCommand
  )
  {
    _getBookByCopyIdQuery = getBookByCopyIdQuery;
    _returnBookCommand = returnBookCommand;
  }

  public async Task HandleAsync(ReturnBookRequest returnBookRequest, Employee employee, long tenantId)
  {
    var bookId = await _getBookByCopyIdQuery.GetBookIdByCopyIdAsync(returnBookRequest.BookCopyId, tenantId);

    var returnBookCommandParams = new ReturnBookCommandParams
    {
      BookCopyId = returnBookRequest.BookCopyId,
      BookId = bookId,
      ProgressOfReading = (ProgressOfReading)Enum.Parse(typeof(ProgressOfReading), returnBookRequest.ProgressOfReading),
      ActualReturnedAtUtc = DateTime.UtcNow,
      Rating = returnBookRequest.Rating,
      Advantages = returnBookRequest.Advantages,
      Disadvantages = returnBookRequest.Disadvantages,
    };

     await _returnBookCommand.ReturnAsync(returnBookCommandParams, employee, tenantId);
  }
}
