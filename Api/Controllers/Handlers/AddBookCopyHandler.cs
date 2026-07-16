using Application.Commands;

namespace Api.Controllers.Handlers;

public class AddBookCopyHandler
{
  private readonly AddBookCopyCommand _addBookCopyCommand;
    
  public AddBookCopyHandler(
    AddBookCopyCommand addBookCopyCommand
  )
  {
    _addBookCopyCommand = addBookCopyCommand;
  }

  public async Task HandleAsync(long bookId, long tenantId)
  {
    var addBookCopyCommandParams = new AddBookCopyCommandParams
    {
      BookId = bookId,
    };

    await _addBookCopyCommand.ExecuteAsync(addBookCopyCommandParams, tenantId);
  }
}
