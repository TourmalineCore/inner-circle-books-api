using Api.Controllers.Handlers;
using Api.Requests;
using Application.Queries;
using Core.Entities;
using Moq;
using Xunit;

namespace Application.Commands;

public class ReturnBookHandlerTests
{
  private const long TENANT_ID = 1;

  private readonly ReturnBookHandler _handler;

  public ReturnBookHandlerTests()
  {
    var getBookByCopyIdQueryMock = new Mock<IGetBookByCopyIdQuery>();

    getBookByCopyIdQueryMock
      .Setup(x => x.GetByCopyIdAsync(1, TENANT_ID))
      .ReturnsAsync(new Book
      {
        Id = 1,
        TenantId = TENANT_ID
      });

    _handler = new ReturnBookHandler(getBookByCopyIdQueryMock.Object, null);
  }

  [Fact]
  public async Task HandleAsyncWithNonExistedBookCopyId_ShouldThrowException()
  {
    var createBookRequest = new ReturnBookRequest
    {
      BookCopyId = 999
    };

    var exception = await Assert.ThrowsAsync<ArgumentException>(
      async () => await _handler.HandleAsync(createBookRequest, null, TENANT_ID)
    );

    Assert.Equal($"Book copy with id {createBookRequest.BookCopyId} not found", exception.Message);
  }
}
