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
    private readonly IDeleteBookCommand _deleteBookCommand;
    private readonly ISoftDeleteBookCommand _softDeleteBookCommand;

    /// <summary>
    ///     Controller with actions to books
    /// </summary>
    public BooksController(
        IGetAllBooksQuery getAllBooksQuery,
        ICreateBookCommand createBookCommand,
        IDeleteBookCommand deleteBookCommand,
        ISoftDeleteBookCommand softDeleteBookCommand
        )
    {
        _getAllBooksQuery = getAllBooksQuery;
        _createBookCommand = createBookCommand;
        _deleteBookCommand = deleteBookCommand;
        _softDeleteBookCommand = softDeleteBookCommand;
    }

    /// <summary>
    ///     Get all books
    /// </summary>
    [HttpGet("all")]
    public async Task<BooksResponse> GetAllToDosAsync()
    {
        var books = await _getAllBooksQuery.GetAllAsync();
        return new BooksResponse
        {
            Books = books.Select(x => new Book
            {
                Id = x.Id,
                Title = x.Title,
                Annotation = x.Annotation,
                ArtworkUrl = x.ArtworkUrl,
                AuthorFullName = x.Author.Name,
                Language = x.Language.ToString(),
                NumberOfCopies = x.NumberOfCopies
            }).ToList()
        };
    }

    /// <summary>
    ///     Adds book
    /// </summary>
    /// <param name="addBookRequest"></param>
    [HttpPost("create")]
    public Task<long> AddToDoAsync([FromBody] AddBookRequest addBookRequest)
    {
        return _createBookCommand.CreateAsync(addBookRequest);
    }

    /// <summary>
    ///     Deletes specific book
    /// </summary>
    /// <param name="id"></param>
    [HttpPost("hard-delete/{id}")]
    public Task DeleteToDo([Required][FromRoute] long id)
    {
        return _deleteBookCommand.DeleteAsync(id);
    }

    /// <summary>
    ///     Soft deletes specific book (mark as deleted, but not deleting from database)
    /// </summary>
    /// <param name="id"></param>
    [HttpPost("soft-delete/{id}")]
    public Task SoftDeleteToDo([FromRoute] long id)
    {
        return _softDeleteBookCommand.SoftDeleteAsync(id);
    }
}