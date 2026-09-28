using Api.Exceptions;
using Api.Responses;
using Application;
using Application.Queries;
using Core;

namespace Api.Controllers.Handlers;

public class GetBookByCopyIdHandler
{
  private readonly IGetBookByCopyIdQuery _getBookByCopyIdQuery;

  private readonly IGetBookByIdQuery _getBookByIdQuery;
  
  private readonly IBookCopyValidatorQuery _bookCopyValidatorQuery;

  private readonly IInnerCircleHttpClient _client;

  public GetBookByCopyIdHandler(
    IGetBookByCopyIdQuery getBookByCopyIdQuery,
    IGetBookByIdQuery getBookByIdQuery,
    IBookCopyValidatorQuery bookCopyValidatorQuery,
    IInnerCircleHttpClient client
  )
  {
    _getBookByCopyIdQuery = getBookByCopyIdQuery;
    _getBookByIdQuery = getBookByIdQuery;
    _bookCopyValidatorQuery = bookCopyValidatorQuery;
    _client = client;
  }

  public async Task<SingleBookResponse> HandleAsync(long copyId, string secretKey, long tenantId)
  {
    var book = await _getBookByCopyIdQuery.GetByCopyIdAsync(copyId, tenantId);

    if (book == null)
    {
      throw new NotFoundException($"Book copy with id {copyId} not found");
    }

    var isSecretKeyValid = await _bookCopyValidatorQuery.IsValidSecretKeyAsync(copyId, secretKey, tenantId);

    if (!isSecretKeyValid)
    {
      throw new ArgumentException("Secret key is not valid");
    }

    var bookCopiesIds = book
      .Copies
      .Select(x => x.Id)
      .ToList();

    var employeesWhoReadNowWithoutFullNames = await _getBookByIdQuery.GetEmployeesWhoReadNowAsync(bookCopiesIds, tenantId);

    var employeesByIds = (!employeesWhoReadNowWithoutFullNames.Any())
      ? new List<EmployeeById>()
      : await _client.GetEmployeesByIdsAsync(employeesWhoReadNowWithoutFullNames
          .Select(x => x.EmployeeId)
          .ToList());

    var employeesDict = employeesByIds.ToDictionary(x => x.EmployeeId);

    var employeesWhoReadNow = (!employeesByIds.Any())
      ? new List<EmployeeWhoReadsNow>()
      : employeesWhoReadNowWithoutFullNames.Select(reader =>
          new EmployeeWhoReadsNow
          {
            EmployeeId = reader.EmployeeId,
            FullName = employeesDict[reader.EmployeeId].FullName,
            BookCopyId = reader.BookCopyId
          })
          .ToList();

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
