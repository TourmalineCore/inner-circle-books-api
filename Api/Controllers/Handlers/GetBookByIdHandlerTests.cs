using Api.Exceptions;
using Application.Queries;
using Core.Entities;
using Moq;
using Xunit;

namespace Api.Controllers.Handlers;

public class GetBookByIdHandlerTests
{
  private const long TENANT_ID = 1;

  [Fact]
  public async Task HandleAsyncWithNonExistedBookId_ShouldThrowNotFoundException()
  {
    var nonExistingBookId = 999;

    var getBookByIdQueryMock = new Mock<IGetBookByIdQuery>();

    getBookByIdQueryMock
      .Setup(x => x.GetByIdAsync(nonExistingBookId, TENANT_ID))
      .ReturnsAsync((Book?)null);

    var getBookByCopyIdHandler = new GetBookByIdHandler(getBookByIdQueryMock.Object, null);

    var exception = await Assert.ThrowsAsync<NotFoundException>(
      async () => await getBookByCopyIdHandler.HandleAsync(nonExistingBookId, TENANT_ID)
    );

    Assert.Equal($"Book with id {nonExistingBookId} not found", exception.Message);
  }
}
