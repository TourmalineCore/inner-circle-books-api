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
    private readonly SoftDeleteBookCommand _softDeleteBookCommand;
    private readonly EditBookCommand _editBookCommand;
    private readonly TakeBookCommand _takeBookCommand;
    private readonly IInnerCircleHttpClient _client;

    /// <summary>
    ///     Controller with actions to books
    /// </summary>
    public BooksController(
        GetAllBooksQuery getAllBooksQuery,
        GetBookByIdQuery getBookByIdQuery,
        CreateBookCommand createBookCommand,
        EditBookCommand editBookCommand,
        DeleteBookCommand deleteBookCommand,
        SoftDeleteBookCommand softDeleteBookCommand,
        TakeBookCommand takeBookCommand,
        IInnerCircleHttpClient client
    )
    {
        _getAllBooksQuery = getAllBooksQuery;
        _getBookByIdQuery = getBookByIdQuery;
        _createBookCommand = createBookCommand;
        _editBookCommand = editBookCommand;
        _deleteBookCommand = deleteBookCommand;
        _softDeleteBookCommand = softDeleteBookCommand;
        _takeBookCommand = takeBookCommand;
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
    public async Task<SingleBookResponse> GetBookByIdAsync([Required][FromRoute] long id)
    {
        var book = await _getBookByIdQuery.GetByIdAsync(id, User.GetTenantId());

        return new SingleBookResponse()
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
            BookCopiesIds = book
                .Copies
                .Select(x => x.Id)
                .ToList(),
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
    public async Task<TakeBookResponse> TakeBookAsync([Required][FromBody] TakeBookRequest takeBookRequest)
    {
        var employee = await _client.GetEmployeeAsync(User.GetCorporateEmail());
        
        var takeBookCommandParams = new TakeBookCommandParams
        {
            BookCopyId = takeBookRequest.BookCopyId,
            SсheduledReturnDate = takeBookRequest.SсheduledReturnDate,
        };

        var newBookCopyReadingHistoryId = await _takeBookCommand.TakeAsync(takeBookCommandParams, employee);

        return new TakeBookResponse()
        {
            NewBookCopyReadingHistoryId = newBookCopyReadingHistoryId
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
            .Select(author => new Author {
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
}