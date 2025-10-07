using System.ComponentModel.DataAnnotations;
using Api.Responses;
using Api.Requests;
using Application;
using Application.Commands;
using Application.Queries;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Filters;
using Core;

namespace Api.Controllers;

/// <summary>
///     Controller with actions to books
/// </summary>
[Authorize]
[Route("api/books")]
public class BooksController : Controller
{
    private readonly CreateBookCommand _createBookCommand;
    private readonly DeleteBookCommand _deleteBookCommand;
    private readonly GetAllBooksQuery _getAllBooksQuery;
    private readonly GetBookByIdQuery _getBookByIdQuery;
    private readonly GetBookByCopyIdQuery _getBookByCopyIdQuery;
    private readonly GetBookHistoryByIdQuery _getBookHistoryByIdQuery;
    private readonly SoftDeleteBookCommand _softDeleteBookCommand;
    private readonly EditBookCommand _editBookCommand;
    private readonly TakeBookCommand _takeBookCommand;
    private readonly ReturnBookCommand _returnBookCommand;
    private readonly IInnerCircleHttpClient _client;

    /// <summary>
    ///     Controller with actions to books
    /// </summary>
    public BooksController(
        GetAllBooksQuery getAllBooksQuery,
        GetBookByIdQuery getBookByIdQuery,
        GetBookByCopyIdQuery getBookByCopyIdQuery,
        GetBookHistoryByIdQuery getBookHistoryByIdQuery,
        CreateBookCommand createBookCommand,
        EditBookCommand editBookCommand,
        DeleteBookCommand deleteBookCommand,
        SoftDeleteBookCommand softDeleteBookCommand,
        TakeBookCommand takeBookCommand,
        ReturnBookCommand returnBookCommand,
        IInnerCircleHttpClient client
    )
    {
        _getAllBooksQuery = getAllBooksQuery;
        _getBookByIdQuery = getBookByIdQuery;
        _getBookByCopyIdQuery = getBookByCopyIdQuery;
        _getBookHistoryByIdQuery = getBookHistoryByIdQuery;
        _createBookCommand = createBookCommand;
        _editBookCommand = editBookCommand;
        _deleteBookCommand = deleteBookCommand;
        _softDeleteBookCommand = softDeleteBookCommand;
        _takeBookCommand = takeBookCommand;
        _returnBookCommand = returnBookCommand;
        _client = client;
    }

    /// <summary>
    ///     Get all books
    /// </summary>
    [RequiresPermission(UserClaimsProvider.CanViewBooks)]
    [HttpGet]
    public async Task<BooksListResponse> GetAllBooksAsync()
    {
        var books = await _getAllBooksQuery.GetAllAsync(User.GetTenantId());
        return new BooksListResponse
        {
            Books = books.Select(x => new BookListItem()
            {
                Id = x.Id,
                Title = x.Title,
                Annotation = x.Annotation,
                CoverUrl = x.CoverUrl,
                Authors = x.Authors.Select(a => new AuthorResponse()
                {
                    FullName = a.FullName
                }).ToList(),
                Language = x.Language.ToString()
            }).ToList()
        };
    }

    /// <summary>
    ///     Get book by id
    /// </summary>
    [RequiresPermission(UserClaimsProvider.CanViewBooks)]
    [HttpGet("{id}")]
    public async Task<ActionResult<SingleBookResponse>> GetBookByIdAsync([Required][FromRoute] long id)
    {
        return await GetBookResponseAsync(id);
    }

    /// <summary>
    ///     Get book by copyId
    /// </summary>
    [RequiresPermission(UserClaimsProvider.CanViewBooks)]
    [HttpGet("copy/{id}")]
    public async Task<ActionResult<SingleBookResponse>> GetBookByCopyIdAsync([Required][FromRoute] long id)
    {
        var bookId = await _getBookByCopyIdQuery.GetBookIdByCopyIdAsync(id);

        if (bookId == null)
        {
            return NotFound(new
            {
                Message = $"Book copy with id {id} not found"
            });
        }

        return await GetBookResponseAsync(bookId);
    }

    /// <summary>
    ///     Adds book
    /// </summary>
    /// <param name="createBookRequest"></param>
    [RequiresPermission(UserClaimsProvider.CanManageBooks)]
    [HttpPost]
    public async Task<CreateBookResponse> CreateBookAsync([Required][FromBody] CreateBookRequest createBookRequest)
    {
        var authors = createBookRequest
            .Authors
            .Select(author => new Author
            {
                FullName = author.FullName,
            })
            .ToList();

        var createBookCommandParams = new CreateBookCommandParams
        {
            Title = createBookRequest.Title,
            Annotation = createBookRequest.Annotation,
            Authors = authors,
            Language = (Language)Enum.Parse(typeof(Language), createBookRequest.Language),
            CoverUrl = createBookRequest.CoverUrl,
            CountOfCopies = createBookRequest.CountOfCopies,
        };

        var newBookId = await _createBookCommand.CreateAsync(createBookCommandParams, User.GetTenantId());

        return new CreateBookResponse()
        {
            NewBookId = newBookId
        };
    }

    /// <summary>
    ///     Take book
    /// </summary>
    /// <param name="takeBookRequest"></param>
    [RequiresPermission(UserClaimsProvider.CanViewBooks)]
    [HttpPost("take")]
    public async Task<IActionResult> TakeBookAsync([Required][FromBody] TakeBookRequest takeBookRequest)
    {
        try
        {
            var employee = await _client.GetEmployeeAsync(User.GetCorporateEmail());

            var takeBookCommandParams = new TakeBookCommandParams
            {
                BookCopyId = takeBookRequest.BookCopyId,
                ScheduledReturnDate = takeBookRequest.ScheduledReturnDate,
            };

            await _takeBookCommand.TakeAsync(takeBookCommandParams, employee);

            return Ok(new { success = true });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    ///     Return book
    /// </summary>
    /// <param name="returnBookRequest"></param>
    [RequiresPermission(UserClaimsProvider.CanViewBooks)]
    [HttpPost("return")]
    public async Task ReturnBookAsync([Required][FromBody] ReturnBookRequest returnBookRequest)
    {

        var employee = await _client.GetEmployeeAsync(User.GetCorporateEmail());

        var returnBookCommandParams = new ReturnBookCommandParams
        {
            BookCopyId = returnBookRequest.BookCopyId,
            ProgressOfReading = (ProgressOfReading)Enum.Parse(typeof(ProgressOfReading), returnBookRequest.ProgressOfReading),
            ActualReturnedAtUtc = DateTime.UtcNow
        };

        await _returnBookCommand.ReturnAsync(returnBookCommandParams, employee);
    }

    /// <summary>
    ///     Get book history by id
    /// </summary>
    [RequiresPermission(UserClaimsProvider.CanViewBooks)]
    [HttpGet("history/{id}")]
    public async Task<BookHistoryResponse> GetBookHistoryByIdAsync([Required][FromRoute] long id)
    {
        var bookHistory = await _getBookHistoryByIdQuery.GetByIdAsync(id);

        var uniqueReaderEmployeeIds = bookHistory
            .Select(x => x.ReaderEmployeeId)
            .Distinct()
            .ToList();

        var employeesByIds = (!uniqueReaderEmployeeIds.Any())
            ? new List<EmployeeById>()
            : await _client.GetEmployeesByIdsAsync(uniqueReaderEmployeeIds);

        var uniqueBookCopyIds = bookHistory
            .Select(x => x.BookCopyId)
            .Distinct()
            .OrderBy(copyId => copyId)
            .ToList();

        var bookCopyIdToCopyNumber = new List<(long BookCopyId, int CopyNumber)>();
        int copyNumber = 1;
        foreach (var bookCopyId in uniqueBookCopyIds)
        {
            bookCopyIdToCopyNumber.Add((bookCopyId, copyNumber));
            copyNumber++;
        }
        
        return new BookHistoryResponse
        {
            List = bookHistory
                .Select(history =>
                {
                    return new BookHistoryItem
                    {
                        CopyNumber = bookCopyIdToCopyNumber.FirstOrDefault(x => x.BookCopyId == history.BookCopyId).CopyNumber,
                        EmployeeFullName = employeesByIds.FirstOrDefault(x => x.EmployeeId == history.ReaderEmployeeId).FullName,
                        TakenDate = history.TakenAtUtc.ToString("dd.MM.yyyy"),
                        ScheduledReturnDate = history.ScheduledReturnDate.ToString("dd.MM.yyyy"),
                        ActualReturnedDate = history.ActualReturnedAtUtc?.ToString("dd.MM.yyyy"),
                        ProgressOfReading = history.ProgressOfReading?.ToString()
                    };
                })
                .ToList(),
            TotalCount = bookHistory.Count
        };
    }

    /// <summary>
    ///     Edit book
    /// </summary>
    /// <param name="id"></param>
    /// <param name="editBookRequest"></param>
    [RequiresPermission(UserClaimsProvider.CanManageBooks)]
    [HttpPost("{id}/edit")]
    public Task EditBook([Required][FromRoute] long id, [Required][FromBody] EditBookRequest editBookRequest)
    {
        var authors = editBookRequest
            .Authors
            .Select(author => new Author
            {
                FullName = author.FullName,
            })
            .ToList();

        var editBookCommandParams = new EditBookCommandParams
        {
            Title = editBookRequest.Title,
            Annotation = editBookRequest.Annotation,
            Authors = authors,
            Language = (Language)Enum.Parse(typeof(Language), editBookRequest.Language),
            CoverUrl = editBookRequest.CoverUrl,
        };

        return _editBookCommand.EditAsync(id, editBookCommandParams, User.GetTenantId());
    }

    /// <summary>
    ///     Deletes specific book
    /// </summary>
    /// <param name="id"></param>
    [RequiresPermission(UserClaimsProvider.IsBooksHardDeleteAllowed)]
    [HttpDelete("{id}/hard-delete")]
    public async Task<object> HardDeleteBook([Required][FromRoute] long id)
    {
        await _deleteBookCommand.DeleteAsync(id, User.GetTenantId());
        return new { isDeleted = true };
    }

    /// <summary>
    ///     Soft deletes specific book (mark as deleted, but not deleting from database)
    /// </summary>
    /// <param name="id"></param>
    [RequiresPermission(UserClaimsProvider.CanManageBooks)]
    [HttpDelete("{id}/soft-delete")]
    public async Task<object> SoftDeleteBook([Required][FromRoute] long id)
    {
        await _softDeleteBookCommand.SoftDeleteAsync(id, User.GetTenantId());
        return new { isDeleted = true };
    }

    private async Task<ActionResult<SingleBookResponse>> GetBookResponseAsync(long id)
    {
        try
        {
            var book = await _getBookByIdQuery.GetByIdAsync(id, User.GetTenantId());

            if (book == null)
            {
                return NotFound(new
                {
                    Message = $"Book with id {id} not found"
                });
            }

            var bookCopiesIds = book
                    .Copies
                    .Select(x => x.Id)
                    .ToList();

            var employeesWhoReadNowWithoutFullNames = await _getBookByIdQuery.GetEmployeesWhoReadNowAsync(bookCopiesIds);

            var employeesByIds = (!employeesWhoReadNowWithoutFullNames.Any())
                ? new List<EmployeeById>()
                : await _client.GetEmployeesByIdsAsync(employeesWhoReadNowWithoutFullNames
                    .Select(x => x.EmployeeId)
                    .ToList());

            var employeesWhoReadNow = (!employeesByIds.Any())
                ? new List<EmployeeWhoReadsNow>()
                : employeesWhoReadNowWithoutFullNames.Select(reader =>
                  {
                      var employee = employeesByIds.FirstOrDefault(x => x.EmployeeId == reader.EmployeeId);

                      return new EmployeeWhoReadsNow
                      {
                          EmployeeId = reader.EmployeeId,
                          FullName = employee.FullName,
                          BookCopyId = reader.BookCopyId
                      };
                  }).ToList();

            var response = new SingleBookResponse()
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
                BookCopiesIds = bookCopiesIds,
                EmployeesWhoReadNow = employeesWhoReadNow
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Exception = ex.Message,
                Stack = ex.StackTrace
            });
        }
    }
}