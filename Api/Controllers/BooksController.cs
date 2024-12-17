using System.ComponentModel.DataAnnotations;
using Api.Responses;
using Application.Commands.Contracts;
using Application.Queries.Contracts;
using Application.Requests;
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
    private readonly ICreateBookCommand _createBookCommand;
    private readonly IDeleteBookCommand _deleteBookCommand;
    private readonly IGetAllBooksQuery _getAllBooksQuery;
    private readonly IGetBookByIdQuery _getBookByIdQuery;
    private readonly ISoftDeleteBookCommand _softDeleteBookCommand;
    private readonly IUpdateBookCommand _updateBookCommand;

    /// <summary>
    ///     Controller with actions to books
    /// </summary>
    public BooksController(
        IGetAllBooksQuery getAllBooksQuery,
        IGetBookByIdQuery getBookByIdQuery,
        ICreateBookCommand createBookCommand,
        IUpdateBookCommand updateBookCommand,
        IDeleteBookCommand deleteBookCommand,
        ISoftDeleteBookCommand softDeleteBookCommand
    )
    {
        _getAllBooksQuery = getAllBooksQuery;
        _getBookByIdQuery = getBookByIdQuery;
        _createBookCommand = createBookCommand;
        _updateBookCommand = updateBookCommand;
        _deleteBookCommand = deleteBookCommand;
        _softDeleteBookCommand = softDeleteBookCommand;
    }

    /// <summary>
    ///     Get all books
    /// </summary>
    [HttpGet("all")]
    public async Task<BooksResponse> GetAllBooksAsync()
    {
        var books = await _getAllBooksQuery.GetAllAsync(User.GetTenantId());
        return new BooksResponse
        {
            Books = books.Select(x => new Book
            {
                Id = x.Id,
                Title = x.Title,
                Annotation = x.Annotation,
                ArtworkUrl = x.ArtworkUrl,
                Authors = x.Authors.Select(a => new Author
                {
                    Id = a.Id, 
                    FullName = a.FullName
                }).ToList(),
                Language = x.Language.ToString()
            }).ToList()
        };
    }

    /// <summary>
    ///     Get book by id
    /// </summary>
    [HttpGet("{id}")]
    public async Task<Book> GetBookByIdAsync([FromRoute] long id)
    {
        var book = await _getBookByIdQuery.GetByIdAsync(id, User.GetTenantId());
        return new Book()
        {
            Id = book.Id,
            Title = book.Title,
            Annotation = book.Annotation,
            ArtworkUrl = book.ArtworkUrl,
            Authors = book.Authors.Select(a => new Author
            {
                Id = a.Id, 
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
    [HttpPost("create")]
    public async Task<CreateBookResponse> CreateBookAsync([FromBody] CreateBookRequest createBookRequest)
    {
        var newBookId = await _createBookCommand.CreateAsync(createBookRequest, User.GetTenantId());
        return new CreateBookResponse()
        {
            NewBookId = newBookId
        };
    }

    /// <summary>
    ///     Update book
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updateBookRequest"></param>
    [RequiresPermission(UserClaimsProvider.CanManageBooks)]
    [HttpPost("{id}/edit")]
    public Task UpdateBook([FromRoute] long id, [FromBody] UpdateBookRequest updateBookRequest)
    {
        return _updateBookCommand.UpdateAsync(id, updateBookRequest, User.GetTenantId());
    }

    /// <summary>
    ///     Deletes specific book
    /// </summary>
    /// <param name="id"></param>
    [RequiresPermission(UserClaimsProvider.IsBooksHardDeleteAllowed, UserClaimsProvider.CanManageBooks)]
    [HttpDelete("{id}/hard-delete")]
    public async Task<object> HardDeleteBook([Required] [FromRoute] long id)
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
    public Task SoftDeleteBook([FromRoute] long id)
    {
        return _softDeleteBookCommand.SoftDeleteAsync(id, User.GetTenantId());
    }
}