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
using Application.Services;

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
    private readonly GetBookCopyReadingHistoryByCopyIdQuery _getBookCopyReadingHistoryByCopyIdQuery;
    private readonly GetBookHistoryByIdQuery _getBookHistoryByIdQuery;
    private readonly BookCopyValidatorQuery _bookCopyValidatorQuery;
    private readonly SoftDeleteBookCommand _softDeleteBookCommand;
    private readonly EditBookCommand _editBookCommand;
    private readonly ReturnBookCommand _returnBookCommand;
    private readonly TakeBookService _takeBookService;
    private readonly IInnerCircleHttpClient _client;

    /// <summary>
    ///     Controller with actions to books
    /// </summary>
    public BooksController(
        GetAllBooksQuery getAllBooksQuery,
        GetBookByIdQuery getBookByIdQuery,
        GetBookByCopyIdQuery getBookByCopyIdQuery,
        GetBookCopyReadingHistoryByCopyIdQuery getBookCopyReadingHistoryByCopyIdQuery,
        GetBookHistoryByIdQuery getBookHistoryByIdQuery,
        BookCopyValidatorQuery bookCopyValidatorQuery,
        CreateBookCommand createBookCommand,
        EditBookCommand editBookCommand,
        DeleteBookCommand deleteBookCommand,
        SoftDeleteBookCommand softDeleteBookCommand,
        ReturnBookCommand returnBookCommand,
        TakeBookService takeBookService,
        IInnerCircleHttpClient client
    )
    {
        _getAllBooksQuery = getAllBooksQuery;
        _getBookByIdQuery = getBookByIdQuery;
        _getBookByCopyIdQuery = getBookByCopyIdQuery;
        _getBookCopyReadingHistoryByCopyIdQuery = getBookCopyReadingHistoryByCopyIdQuery;
        _getBookHistoryByIdQuery = getBookHistoryByIdQuery;
        _bookCopyValidatorQuery = bookCopyValidatorQuery;
        _createBookCommand = createBookCommand;
        _editBookCommand = editBookCommand;
        _deleteBookCommand = deleteBookCommand;
        _softDeleteBookCommand = softDeleteBookCommand;
        _returnBookCommand = returnBookCommand;
        _takeBookService = takeBookService;
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
    public async Task<ActionResult<SingleBookResponse>> GetBookByCopyIdAsync([Required][FromRoute] long id, [Required][FromQuery] string secretKey)
    {
        var bookId = await _getBookByCopyIdQuery.GetBookIdByCopyIdAsync(id, User.GetTenantId());

        if (bookId == 0)
        {
            return NotFound(new
            {
                Message = $"Book copy with id {id} not found"
            });
        }

        var isSecretKeyValid = await _bookCopyValidatorQuery.IsValidSecretKeyAsync(id, secretKey, User.GetTenantId());

        if (!isSecretKeyValid) {
            return NotFound(new
            {
                Message = "Secret key is not valid"
            });
        }


        return await GetBookResponseAsync(bookId);
    }


    /// <summary>
    ///     Get book with copies by id
    /// </summary>
    [RequiresPermission(UserClaimsProvider.CanManageBooks)]
    [HttpGet("copies/{id}")]
    public async Task<ActionResult<BookWithCopiesResponse>> GetBookCopiesByIdAsync([Required][FromRoute] long id)
    {
        var book = await _getBookByIdQuery.GetByIdAsync(id, User.GetTenantId());

        if (book == null)
        {
            return NotFound(new
            {
                Message = $"Book with id {id} not found"
            });
        }

        var bookCopies = book
            .Copies
            .Select(copy => new BookCopyResponse
            {
                BookCopyId = copy.Id,
                SecretKey = copy.SecretKey
            })
            .ToList();

        return new BookWithCopiesResponse()
        {
            BookTitle = book.Title,
            BookCopies = bookCopies
        };
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

            var returnBookCommandParams = new ReturnBookCommandParams
            {
                BookCopyId = takeBookRequest.BookCopyId,
                ProgressOfReading = ProgressOfReading.Unknown,
                ActualReturnedAtUtc = DateTime.UtcNow
            };

            var activeReading = await _getBookCopyReadingHistoryByCopyIdQuery.GetActiveReadingAsync(returnBookCommandParams.BookCopyId, User.GetTenantId());

            await _takeBookService.TakeAsync(takeBookCommandParams, returnBookCommandParams, employee, User.GetTenantId(), activeReading);

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

        await _returnBookCommand.ReturnAsync(returnBookCommandParams, employee, User.GetTenantId());
    }

    /// <summary>
    ///     Get book history by id
    /// </summary>
    [RequiresPermission(UserClaimsProvider.CanViewBooks)]
    [HttpGet("history/{id}")]
    public async Task<BookHistoryResponse> GetBookHistoryByIdAsync(
        [Required][FromRoute] long id,
        [FromQuery] int page,
        [FromQuery] int pageSize
    )
    {
        var (bookHistory, totalCount) = await _getBookHistoryByIdQuery.GetByIdAsync(id, page, pageSize, User.GetTenantId());

        var uniqueReaderEmployeeIds = bookHistory
            .Select(x => x.ReaderEmployeeId)
            .Distinct()
            .ToList();

        var employeesByIds = (!uniqueReaderEmployeeIds.Any())
            ? new List<EmployeeById>()
            : await _client.GetEmployeesByIdsAsync(uniqueReaderEmployeeIds);

        return new BookHistoryResponse
        {
            List = bookHistory
                .Select(history =>
                {
                    return new BookHistoryItem
                    {
                        BookCopyId = history.BookCopyId,
                        EmployeeFullName = employeesByIds.FirstOrDefault(x => x.EmployeeId == history.ReaderEmployeeId).FullName,
                        TakenDate = history.TakenAtUtc.ToString("dd.MM.yyyy"),
                        ScheduledReturnDate = history.ScheduledReturnDate.ToString("dd.MM.yyyy"),
                        ActualReturnedDate = history.ActualReturnedAtUtc?.ToString("dd.MM.yyyy"),
                        ProgressOfReading = history.ProgressOfReading?.ToString()
                    };
                })
                .ToList(),
            TotalCount = totalCount
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

            var employeesWhoReadNowWithoutFullNames = await _getBookByIdQuery.GetEmployeesWhoReadNowAsync(bookCopiesIds, User.GetTenantId());

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