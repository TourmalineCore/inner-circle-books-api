using Api.Responses;
using Application.Queries;
using Application.Services;
using Core;

namespace Api.Controllers.Handlers;

public class GetAllBooksHandler
{
  private readonly GetAllBooksQuery _getAllBooksQuery;

  private readonly IBookReadersService _bookReadersService;

  public GetAllBooksHandler(
    GetAllBooksQuery getAllBooksQuery,
    IBookReadersService bookReadersService
  )
  {
    _getAllBooksQuery = getAllBooksQuery;
    _bookReadersService = bookReadersService;
  }

  public async Task<BooksListResponse> HandleAsync(Employee employee)
  {
    var books = await _getAllBooksQuery.GetAllAsync(employee.TenantId);

    var allBookCopiesIds = books
      .SelectMany(x => x.Copies.Select(x => x.Id))
      .ToList();

    var allEmployeesWhoReadNow = await _bookReadersService.GetEmployeesWhoReadNowAsync(allBookCopiesIds, employee.TenantId);

    var readersGroupedByBookCopyId = allEmployeesWhoReadNow
      .GroupBy(x => x.BookCopyId)
      .ToDictionary(
        group => group.Key,
        group => group.ToList()
      );

    var bookItems = books.Select(book =>
    {
      var bookCopiesIds = book.Copies
        .Select(x => x.Id)
        .ToList();

      var employeesWhoReadNow = bookCopiesIds
        .Where(bookCopyId => readersGroupedByBookCopyId.ContainsKey(bookCopyId))
        .SelectMany(bookCopyId => readersGroupedByBookCopyId[bookCopyId])
        .ToList();

      var availabilityStatuses = AvailabilityStatusCalculator.Calculate(
        bookCopiesIds.Count,
        employeesWhoReadNow,
        employee.Id
      );

      return new BookListItem
      {
        Id = book.Id,
        Title = book.Title,
        Annotation = book.Annotation,
        CoverUrl = book.CoverUrl,
        Authors = book.Authors
          .Select(x => new AuthorResponse
          {
            FullName = x.FullName
          })
          .ToList(),
        Language = book.Language.ToString(),
        KnowledgeAreas = book.KnowledgeAreas
          .Select(x => new KnowledgeAreaItem
          { 
            Id = x.Id,
            Name = x.Name
          })
          .ToList(),
        AvailabilityStatuses = availabilityStatuses
          .Select(x => x.ToString())
          .ToList()
      };
    })
    .ToList();

    return new BooksListResponse
    {
      Books = bookItems
    };
  }
}
