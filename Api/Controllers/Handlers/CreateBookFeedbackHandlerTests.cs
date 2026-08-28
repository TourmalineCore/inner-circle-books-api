using Api.Controllers.Handlers;
using Api.Requests;
using Core.Entities;
using Xunit;

namespace Application.Commands;

public class CreateBookFeedbackHandlerTests
{
  private const long TENANT_ID = 1;

  [Theory]
  [InlineData(ProgressOfReading.NotReadAtAll)]
  [InlineData(ProgressOfReading.Unknown)]
  public async Task HandleAsyncWithNotAllowedProgressOfReadingStatus_ShouldThrowException(ProgressOfReading progressOfReading)
  {
    var createBookFeedbackRequest = new CreateBookFeedbackRequest
    {
      ProgressOfReading = progressOfReading.ToString(),
    };

    var createBookFeedbackHandler = new CreateBookFeedbackHandler(null);

    var exception = await Assert.ThrowsAsync<ArgumentException>(
      async () => await createBookFeedbackHandler.HandleAsync(1, createBookFeedbackRequest, null, TENANT_ID)
    );

    Assert.Equal($"Only {ProgressOfReading.ReadPartially} and {ProgressOfReading.ReadEntirely} are allowed.", exception.Message);
  }
}
