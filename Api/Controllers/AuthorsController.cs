using System.ComponentModel.DataAnnotations;
using Application.Commands.Contracts;
using Application.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Filters;

namespace Api.Controllers;

/// <summary>
///     Controller with actions to authors
/// </summary>
[Authorize]
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
    [RequiresPermission(UserClaimsProvider.CanManageBooks)]
    [HttpPost("{id}/edit")]
    public Task UpdateAuthor([Required] [FromRoute] long id, [FromBody] UpdateAuthorRequest updateAuthorRequest)
    {
        return _updateAuthorCommand.UpdateAsync(id, updateAuthorRequest, User.GetTenantId());
    }

    /// <summary>
    ///     Deletes specific author
    /// </summary>
    /// <param name="id"></param>
    [RequiresPermission(UserClaimsProvider.CanManageBooks)]
    [HttpDelete("{id}/hard-delete")]
    public async Task<object> HardDeleteAuthor([Required][FromRoute] long id)
    {
        await _deleteAuthorCommand.DeleteAsync(id, User.GetTenantId());
        return new { isDeleted = true };
    }
}