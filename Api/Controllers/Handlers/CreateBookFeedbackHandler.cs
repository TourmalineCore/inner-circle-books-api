using Api.Requests;
using Api.Responses;
using Application.Commands;
using Core;
using Core.Entities;

namespace Api.Controllers.Handlers;

public class CreateBookFeedbackHandler
{
  private readonly CreateBookFeedbackCommand _createBookFeedbackCommand;
    
  public CreateBookFeedbackHandler(
    CreateBookFeedbackCommand createBookFeedbackCommand
  )
  {
    _createBookFeedbackCommand = createBookFeedbackCommand;
  }

  public async Task<CreateBookFeedbackResponse> HandleAsync(
    long bookId,
    CreateBookFeedbackRequest createBookFeedbackRequest,
    Employee employee,
    long tenantId
  )
  {
    var createBookFeedbackCommandParams = new CreateBookFeedbackCommandParams
    {
      BookId = bookId,
      ProgressOfReading = (ProgressOfReading)Enum.Parse(typeof(ProgressOfReading), createBookFeedbackRequest.ProgressOfReading),
      Rating = createBookFeedbackRequest.Rating,
      Advantages = createBookFeedbackRequest.Advantages,
      Disadvantages = createBookFeedbackRequest.Disadvantages
    };

    var newBookFeedbackId = await _createBookFeedbackCommand.ExecuteAsync(createBookFeedbackCommandParams, employee, tenantId);

    return new CreateBookFeedbackResponse()
    {
      NewBookFeedbackId = newBookFeedbackId
    };
  }
}
