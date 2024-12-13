using System.ComponentModel.DataAnnotations;
using Api.Responses;
using Application.Commands.Contracts;
using Application.Queries.Contracts;
using Application.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
///     Controller with actions to books
/// </summary>
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
    /// <param name="addBookRequest"></param>
    [HttpPost("create")]
    public Task<long> AddBookAsync([FromBody] AddBookRequest addBookRequest)
    {
        return _createBookCommand.CreateAsync(addBookRequest, User.GetTenantId());
    }

    /// <summary>
    ///     Update book
    /// </summary>
    /// <param name="addBookRequest"></param>
    [HttpPost("{id}/edit")]
    public Task UpdateBook([FromRoute] long id, [FromBody] UpdateBookRequest updateBookRequest)
    {
        return _updateBookCommand.UpdateAsync(id, updateBookRequest, User.GetTenantId());
    }

    /// <summary>
    ///     Deletes specific book
    /// </summary>
    /// <param name="id"></param>
    [HttpDelete("{id}/hard-delete")]
    public Task HardDeleteBook([Required] [FromRoute] long id)
    {
        return _deleteBookCommand.DeleteAsync(id, User.GetTenantId());
    }

    /// <summary>
    ///     Soft deletes specific book (mark as deleted, but not deleting from database)
    /// </summary>
    /// <param name="id"></param>
    [HttpDelete("{id}/soft-delete")]
    public Task SoftDeleteBook([FromRoute] long id)
    {
        return _softDeleteBookCommand.SoftDeleteAsync(id, User.GetTenantId());
    }
}