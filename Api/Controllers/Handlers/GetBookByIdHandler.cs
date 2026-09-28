using Api.Exceptions;
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
