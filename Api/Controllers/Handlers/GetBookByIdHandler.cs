using Api.Exceptions;
using Api.Mappers;
using Api.Responses;
using Application.Queries;
using Application.Services;
using Core;

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

  public async Task<SingleBookResponse> HandleAsync(
    long bookId,
    Employee employee
  )
  {
    var book = await _getBookByIdQuery.GetByIdAsync(bookId, employee.TenantId);

    if (book == null)
    {
      throw new NotFoundException($"Book with id {bookId} not found");
    }

    var bookCopiesIds = book
      .Copies
      .Select(x => x.Id)
      .ToList();

    var employeesWhoReadNow = await _bookReadersService.GetEmployeesWhoReadNowAsync(bookCopiesIds, employee.TenantId);

    var availabilityStatuses = BookAvailabilityStatusCalculator.Calculate(
      bookCopiesIds.Count,
      employeesWhoReadNow,
      employee.Id
    );

    return SingleBookResponseMapper.Map(
      book,
      bookCopiesIds,
      employeesWhoReadNow,
      availabilityStatuses
    );
  }
}
