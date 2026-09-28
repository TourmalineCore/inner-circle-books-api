using Api.Exceptions;
using Application.Queries;
using Application.Services;
using Core.Entities;
using Moq;
using Xunit;

namespace Api.Controllers.Handlers;

public class GetBookByCopyIdHandlerTests
{
  private const long TENANT_ID = 1;

  [Fact]
  public async Task HandleAsyncWithNonExistedBookCopyId_ShouldThrowNotFoundException()
  {
    var nonExistingBookCopyId = 999;

    var getBookByCopyIdQueryMock = new Mock<IGetBookByCopyIdQuery>();
    var bookCopyValidatorQueryMock = new Mock<IBookCopyValidatorQuery>();
    var bookReaderServiceMock = new Mock<IBookReadersService>();

    getBookByCopyIdQueryMock
      .Setup(x => x.GetByCopyIdAsync(nonExistingBookCopyId, TENANT_ID))
      .ReturnsAsync((Book?)null);

    var getBookByCopyIdHandler = new GetBookByCopyIdHandler(
      getBookByCopyIdQueryMock.Object,
      bookCopyValidatorQueryMock.Object,
      bookReaderServiceMock.Object
    );

    var exception = await Assert.ThrowsAsync<NotFoundException>(
      async () => await getBookByCopyIdHandler.HandleAsync(nonExistingBookCopyId, "", TENANT_ID)
    );

    Assert.Equal($"Book copy with id {nonExistingBookCopyId} not found", exception.Message);
  }

  [Fact]
  public async Task HandleAsyncWithIvalidSecretKey_ShouldThrowArgumentException()
  {
    var bookCopyId = 1;
    var invalidSecretKey = "invalidSecretKey";

    var getBookByCopyIdQueryMock = new Mock<IGetBookByCopyIdQuery>();
    var bookCopyValidatorQueryMock = new Mock<IBookCopyValidatorQuery>();
    var bookReaderServiceMock = new Mock<IBookReadersService>();

    getBookByCopyIdQueryMock
      .Setup(x => x.GetByCopyIdAsync(bookCopyId, TENANT_ID))
      .ReturnsAsync(
        new Book
        {
          Id = 1,
        }
      );

    bookCopyValidatorQueryMock
      .Setup(x => x.IsValidSecretKeyAsync(bookCopyId, invalidSecretKey, TENANT_ID))
      .ReturnsAsync(false);

    var getBookByCopyIdHandler = new GetBookByCopyIdHandler(
      getBookByCopyIdQueryMock.Object,
      bookCopyValidatorQueryMock.Object,
      bookReaderServiceMock.Object
    );

    var exception = await Assert.ThrowsAsync<ArgumentException>(
      async () => await getBookByCopyIdHandler.HandleAsync(bookCopyId, invalidSecretKey, TENANT_ID)
    );

    Assert.Equal("Secret key is not valid", exception.Message);
  }
}
