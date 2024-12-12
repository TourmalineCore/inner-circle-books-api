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
    private readonly IGetAllBooksQuery _getAllBooksQuery;
    private readonly ICreateBookCommand _createBookCommand;
    private readonly IUpdateBookCommand _updateBookCommand;
    private readonly IDeleteBookCommand _deleteBookCommand;
    private readonly ISoftDeleteBookCommand _softDeleteBookCommand;

    /// <summary>
    ///     Controller with actions to books
    /// </summary>
    public BooksController(
        IGetAllBooksQuery getAllBooksQuery,
        ICreateBookCommand createBookCommand,
        IUpdateBookCommand updateBookCommand,
        IDeleteBookCommand deleteBookCommand,
        ISoftDeleteBookCommand softDeleteBookCommand
        )
    {
        _getAllBooksQuery = getAllBooksQuery;
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
                Authors = x.Authors.Select(a => new Author() { FullName = a.FullName }).ToList(),
                Language = x.Language.ToString(),
            }).ToList()
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
    [HttpPost("edit")]
    public Task UpdateBook([FromBody] UpdateBookRequest updateBookRequest)
    {
        return _updateBookCommand.UpdateAsync(updateBookRequest, User.GetTenantId());
    }

    /// <summary>
    ///     Deletes specific book
    /// </summary>
    /// <param name="id"></param>
    [HttpPost("hard-delete/{id}")]
    public Task HardDeleteBook([Required][FromRoute] long id)
    {
        return _deleteBookCommand.DeleteAsync(id,User.GetTenantId());
    }

    /// <summary>
    ///     Soft deletes specific book (mark as deleted, but not deleting from database)
    /// </summary>
    /// <param name="id"></param>
    [HttpPost("soft-delete/{id}")]
    public Task SoftDeleteBook([FromRoute] long id)
    {
        return _softDeleteBookCommand.SoftDeleteAsync(id, User.GetTenantId());
    }
}