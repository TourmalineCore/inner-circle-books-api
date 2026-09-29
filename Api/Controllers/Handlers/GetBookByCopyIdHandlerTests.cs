using Api.Exceptions;
using Application.Queries;
using Application.Services;
using Core;
using Core.Entities;
using Moq;
using Xunit;

namespace Api.Controllers.Handlers;

public class GetBookByCopyIdHandlerTests
{
  private const long TENANT_ID = 1;

  [Fact]
  public async Task HandleAsyncWithNonExistentBookId_ShouldThrowNotFoundException()
  {
    var nonExistentBookCopyId = 999;
    var employee = new Employee
    {
      Id = 1,
      TenantId = TENANT_ID
    };

    var getBookByCopyIdQueryMock = new Mock<IGetBookByCopyIdQuery>();
    var bookCopyValidatorQueryMock = new Mock<IBookCopyValidatorQuery>();
    var bookReaderServiceMock = new Mock<IBookReadersService>();

    getBookByCopyIdQueryMock
      .Setup(x => x.GetByCopyIdAsync(nonExistentBookCopyId, TENANT_ID))
      .ReturnsAsync((Book?)null);
    
    bookCopyValidatorQueryMock
      .Setup(x => x.IsValidSecretKeyAsync(It.IsAny<long>(), It.IsAny<string>(), TENANT_ID))
      .ReturnsAsync(true);

    var getBookByCopyIdHandler = new GetBookByCopyIdHandler(
      getBookByCopyIdQueryMock.Object,
      bookCopyValidatorQueryMock.Object,
      bookReaderServiceMock.Object
    );

    var exception = await Assert.ThrowsAsync<NotFoundException>(
      async () => await getBookByCopyIdHandler.HandleAsync(nonExistentBookCopyId, "", employee)
    );

    Assert.Equal($"Book copy with id {nonExistentBookCopyId} not found", exception.Message);
  }

  [Fact]
  public async Task HandleAsyncWithInvalidSecretKey_ShouldThrowForbiddenException()
  {
    var bookCopyId = 1;
    var invalidSecretKey = "invalidSecretKey";
    var employee = new Employee
    {
      Id = 1,
      TenantId = TENANT_ID
    };

    var getBookByCopyIdQueryMock = new Mock<IGetBookByCopyIdQuery>();
    var bookCopyValidatorQueryMock = new Mock<IBookCopyValidatorQuery>();
    var bookReaderServiceMock = new Mock<IBookReadersService>();

    bookCopyValidatorQueryMock
      .Setup(x => x.IsValidSecretKeyAsync(bookCopyId, invalidSecretKey, TENANT_ID))
      .ReturnsAsync(false);

    var getBookByCopyIdHandler = new GetBookByCopyIdHandler(
      getBookByCopyIdQueryMock.Object,
      bookCopyValidatorQueryMock.Object,
      bookReaderServiceMock.Object
    );

    var exception = await Assert.ThrowsAsync<ForbiddenException>(
      async () => await getBookByCopyIdHandler.HandleAsync(bookCopyId, invalidSecretKey, employee)
    );

    Assert.Equal("Secret key is not valid", exception.Message);
  }
}
