using Api.Exceptions;
using Api.Mappers;
using Api.Responses;
using Application.Queries;
using Application.Services;

namespace Api.Controllers.Handlers;

public class GetBookByCopyIdHandler
{
  private readonly IGetBookByCopyIdQuery _getBookByCopyIdQuery;

  private readonly IBookCopyValidatorQuery _bookCopyValidatorQuery;

  private readonly IBookReadersService _bookReadersService;

  public GetBookByCopyIdHandler(
    IGetBookByCopyIdQuery getBookByCopyIdQuery,
    IBookCopyValidatorQuery bookCopyValidatorQuery,
    IBookReadersService bookReadersService
  )
  {
    _getBookByCopyIdQuery = getBookByCopyIdQuery;
    _bookCopyValidatorQuery = bookCopyValidatorQuery;
    _bookReadersService = bookReadersService;
  }

  public async Task<SingleBookResponse> HandleAsync(
    long copyId,
    string secretKey,
    long tenantId
  )
  {
    var isSecretKeyValid = await _bookCopyValidatorQuery.IsValidSecretKeyAsync(copyId, secretKey, tenantId);

    if (!isSecretKeyValid)
    {
      throw new ForbiddenException("Secret key is not valid");
    }

    var book = await _getBookByCopyIdQuery.GetByCopyIdAsync(copyId, tenantId);

    if (book == null)
    {
      throw new NotFoundException($"Book copy with id {copyId} not found");
    }

    var bookCopiesIds = book
      .Copies
      .Select(x => x.Id)
      .ToList();

    var employeesWhoReadNow = await _bookReadersService.GetEmployeesWhoReadNowAsync(bookCopiesIds, tenantId);

    return SingleBookResponseMapper.Map(book, bookCopiesIds, employeesWhoReadNow);
  }
}
