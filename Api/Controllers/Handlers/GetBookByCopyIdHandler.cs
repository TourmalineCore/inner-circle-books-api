using Api.Exceptions;
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
    var book = await _getBookByCopyIdQuery.GetByCopyIdAsync(copyId, tenantId);

    if (book == null)
    {
      throw new NotFoundException($"Book copy with id {copyId} not found");
    }

    var isSecretKeyValid = await _bookCopyValidatorQuery.IsValidSecretKeyAsync(copyId, secretKey, tenantId);

    if (!isSecretKeyValid)
    {
      throw new ForbiddenException("Secret key is not valid");
    }

    var bookCopiesIds = book
      .Copies
      .Select(x => x.Id)
      .ToList();

    var employeesWhoReadNow = await _bookReadersService.GetEmployeesWhoReadNowAsync(bookCopiesIds, tenantId);

    return new SingleBookResponse
    {
        Id = book.Id,
        Title = book.Title,
        Annotation = book.Annotation,
        CoverUrl = book.CoverUrl,
        Authors = book
            .Authors
            .Select(a => new AuthorResponse()
            {
              FullName = a.FullName
            })
            .ToList(),
        Language = book.Language.ToString(),
        KnowledgeAreas = book
            .KnowledgeAreas
            .Select(k => new KnowledgeAreaItem
            {
                Id = k.Id,
                Name = k.Name
            })
            .ToList(),
        BookCopiesIds = bookCopiesIds,
        EmployeesWhoReadNow = employeesWhoReadNow
    };
  }
}
