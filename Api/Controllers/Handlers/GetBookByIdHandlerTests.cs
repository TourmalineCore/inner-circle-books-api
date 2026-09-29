using Api.Exceptions;
using Application.Queries;
using Application.Services;
using Core;
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
    var employee = new Employee
    {
      Id = 1,
      TenantId = TENANT_ID
    };

    var getBookByIdQueryMock = new Mock<IGetBookByIdQuery>();
    var bookReadersServicMock = new Mock<IBookReadersService>();

    getBookByIdQueryMock
      .Setup(x => x.GetByIdAsync(nonExistentBookId, TENANT_ID))
      .ReturnsAsync((Book?)null);

    var getBookByCopyIdHandler = new GetBookByIdHandler(getBookByIdQueryMock.Object, bookReadersServicMock.Object);

    var exception = await Assert.ThrowsAsync<NotFoundException>(
      async () => await getBookByCopyIdHandler.HandleAsync(nonExistentBookId, employee)
    );

    Assert.Equal($"Book with id {nonExistentBookId} not found", exception.Message);
  }
}
