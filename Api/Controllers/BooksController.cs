using System.ComponentModel.DataAnnotations;
using Api.Responses;
using Application.Commands;
using Application.Queries.Contracts;
using Api.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Filters;
using Core.Entities;

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
    private readonly IGetAllBooksQuery _getAllBooksQuery;
    private readonly IGetBookByIdQuery _getBookByIdQuery;
    private readonly SoftDeleteBookCommand _softDeleteBookCommand;
    private readonly EditBookCommand _editBookCommand;

    /// <summary>
    ///     Controller with actions to books
    /// </summary>
    public BooksController(
        IGetAllBooksQuery getAllBooksQuery,
        IGetBookByIdQuery getBookByIdQuery,
        CreateBookCommand createBookCommand,
        EditBookCommand editBookCommand,
        DeleteBookCommand deleteBookCommand,
        SoftDeleteBookCommand softDeleteBookCommand
    )
    {
        _getAllBooksQuery = getAllBooksQuery;
        _getBookByIdQuery = getBookByIdQuery;
        _createBookCommand = createBookCommand;
        _editBookCommand = editBookCommand;
        _deleteBookCommand = deleteBookCommand;
        _softDeleteBookCommand = softDeleteBookCommand;
    }

    /// <summary>
    ///     Get all books
    /// </summary>
    // TODO add new permission to account management
    //[RequiresPermission(UserClaimsProvider.CanViewBooks)]
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
                BookCoverUrl = x.BookCoverUrl,
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
    //[RequiresPermission(UserClaimsProvider.CanViewBooks)]
    [HttpGet("{id}")]
    public async Task<SingleBookResponse> GetBookByIdAsync([Required][FromRoute] long id)
    {
        var book = await _getBookByIdQuery.GetByIdAsync(id, User.GetTenantId());
        return new SingleBookResponse()
        {
            Id = book.Id,
            Title = book.Title,
            Annotation = book.Annotation,
            BookCoverUrl = book.BookCoverUrl,
            Authors = book.Authors.Select(a => new AuthorResponse()
            {
                FullName = a.FullName
            }).ToList(),
            Language = book.Language.ToString()
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
        var authors = createBookRequest.Authors.Select(author => new Author
        {
            FullName = author.FullName,
        }).ToList();

        var createBookCommandParams = new CreateBookCommandParams
        {
            Title = createBookRequest.Title,
            Annotation = createBookRequest.Annotation,
            Authors = authors,
            Language = createBookRequest.Language,
            BookCoverUrl = createBookRequest.BookCoverUrl,
        };

        var newBookId = await _createBookCommand.CreateAsync(createBookCommandParams, User.GetTenantId());

        return new CreateBookResponse()
        {
            NewBookId = newBookId
        };
    }

    /// <summary>
    ///     Edit book
    /// </summary>
    /// <param name="id"></param>
    /// <param name="editBookRequest"></param>
    [RequiresPermission(UserClaimsProvider.CanManageBooks)]
    [HttpPost("{id}/edit")]
    // TODO naming is different, need to leave or edit or update
    public Task EditBook([Required][FromRoute] long id, [Required][FromBody] EditBookRequest editBookRequest)
    {
        var authors = editBookRequest.Authors.Select(author => new Author
        {
            FullName = author.FullName,
        }).ToList();

        var editBookCommandParams = new EditBookCommandParams
        {
            Title = editBookRequest.Title,
            Annotation = editBookRequest.Annotation,
            Authors = authors,
            Language = editBookRequest.Language,
            BookCoverUrl = editBookRequest.BookCoverUrl,
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