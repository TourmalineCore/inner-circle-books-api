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
    var nonExistentBookId = 999;

    var getBookByIdQueryMock = new Mock<IGetBookByIdQuery>();

    getBookByIdQueryMock
      .Setup(x => x.GetByIdAsync(nonExistentBookId, TENANT_ID))
      .ReturnsAsync((Book?)null);

    var getBookByCopyIdHandler = new GetBookByIdHandler(getBookByIdQueryMock.Object, null);

    var exception = await Assert.ThrowsAsync<NotFoundException>(
      async () => await getBookByCopyIdHandler.HandleAsync(nonExistentBookId, TENANT_ID)
    );

    Assert.Equal($"Book with id {nonExistentBookId} not found", exception.Message);
  }
}
