using System.ComponentModel.DataAnnotations;
using Application.Commands.Contracts;
using Application.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
///     Controller with actions to authors
/// </summary>
[Route("api/authors")]
public class AuthorsController : Controller
{
    private readonly IDeleteAuthorCommand _deleteAuthorCommand;
    private readonly IUpdateAuthorCommand _updateAuthorCommand;

    /// <summary>
    ///     Controller with actions to authors
    /// </summary>
    public AuthorsController(
        IUpdateAuthorCommand updateAuthorCommand,
        IDeleteAuthorCommand deleteAuthorCommand
    )
    {
        _updateAuthorCommand = updateAuthorCommand;
        _deleteAuthorCommand = deleteAuthorCommand;
    }

    /// <summary>
    ///     Update author
    /// </summary>
    /// <param name="updateAuthorRequest"></param>
    [HttpPost("{id}/edit")]
    public Task UpdateAuthor([Required] [FromRoute] long id, [FromBody] UpdateAuthorRequest updateAuthorRequest)
    {
        return _updateAuthorCommand.UpdateAsync(id, updateAuthorRequest, User.GetTenantId());
    }

    /// <summary>
    ///     Deletes specific author
    /// </summary>
    /// <param name="id"></param>
    [HttpDelete("{id}/hard-delete")]
    public async Task<object> HardDeleteAuthor([Required][FromRoute] long id)
    {
        await _deleteAuthorCommand.DeleteAsync(id, User.GetTenantId());
        return new { isDeleted = true };
    }
}