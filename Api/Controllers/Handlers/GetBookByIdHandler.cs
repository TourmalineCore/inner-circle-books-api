using Api.Exceptions;
using Api.Mappers;
using Api.Responses;
using Application.Queries;
using Application.Services;

namespace Api.Controllers.Handlers;

public class GetBookByIdHandler
{
  private readonly IGetBookByIdQuery _getBookByIdQuery;

  private readonly IBookReadersService _bookReadersService;

  public GetBookByIdHandler(
    IGetBookByIdQuery getBookByIdQuery,
    IBookReadersService bookReadersService
  )
  {
    _getBookByIdQuery = getBookByIdQuery;
    _bookReadersService = bookReadersService;
  }

  public async Task<SingleBookResponse> HandleAsync(long bookId, long tenantId)
  {
    var book = await _getBookByIdQuery.GetByIdAsync(bookId, tenantId);

    if (book == null)
    {
      throw new NotFoundException($"Book with id {bookId} not found");
    }

    var bookCopiesIds = book
      .Copies
      .Select(x => x.Id)
      .ToList();

    var employeesWhoReadNow = await _bookReadersService.GetEmployeesWhoReadNowAsync(bookCopiesIds, tenantId);

    return SingleBookResponseMapper.Map(book, bookCopiesIds, employeesWhoReadNow);
  }
}
