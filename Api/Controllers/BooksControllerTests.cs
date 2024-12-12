using Api.Responses;
using Application.Commands.Contracts;
using Application.Queries.Contracts;
using Application.Requests;
using Application.Services;
using Core.Entities;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Api.Controllers
{
    public class BooksControllerTests
    {
        private readonly Mock<IGetAllBooksQuery> _getAllBooksQueryMock;
        private readonly Mock<ICreateBookCommand> _createBookCommandMock;
        private readonly Mock<IUpdateBookCommand> _updateBookCommandMock;
        private readonly Mock<IDeleteBookCommand> _deleteBookCommandMock;
        private readonly Mock<ISoftDeleteBookCommand> _softDeleteBookCommandMock;
        private readonly BooksController _controller;

        private const long TENANT_ID = 1L;

        public BooksControllerTests()
        {
            _getAllBooksQueryMock = new Mock<IGetAllBooksQuery>();
            _createBookCommandMock = new Mock<ICreateBookCommand>();
            _updateBookCommandMock = new Mock<IUpdateBookCommand>();
            _deleteBookCommandMock = new Mock<IDeleteBookCommand>();
            _softDeleteBookCommandMock = new Mock<ISoftDeleteBookCommand>();

            var claims = new[]
            {
                new Claim("TenantId", TENANT_ID.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(ctx => ctx.User).Returns(claimsPrincipal);

            _controller = new BooksController(
                _getAllBooksQueryMock.Object,
                _createBookCommandMock.Object,
                _updateBookCommandMock.Object,
                _deleteBookCommandMock.Object,
                _softDeleteBookCommandMock.Object
            )
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                }
            };
        }

        [Fact]
        public async Task GetAllBooksAsync_ShouldReturnBooksResponse()
        {
            var books = new List<Core.Entities.Book>
            {
                new ()
                {
                    Id = 1L,
                    TenantId = TENANT_ID,
                    Title = "Test Book 1",
                    Annotation = "Test annotation 1",
                    ArtworkUrl = "http://test1-images.com/img404.png",
                    Authors = new List<Core.Entities.Author>()
                    {
                        new Core.Entities.Author(TENANT_ID, "Test Author 1")
                    },
                },
                new()
                {
                    Id = 2L,
                    TenantId = TENANT_ID,
                    Title = "Test Book 2",
                    Annotation = "Test annotation 2",
                    ArtworkUrl = "http://test2-images.com/img404.png",
                    Authors = new List<Core.Entities.Author>()
                    {
                        new Core.Entities.Author(TENANT_ID, "Test Author 1")
                    },
                }
            };
            _getAllBooksQueryMock
                .Setup(query => query.GetAllAsync(TENANT_ID))
                .ReturnsAsync(books);

            var result = await _controller.GetAllBooksAsync();

            Assert.IsType<BooksResponse>(result);
            Assert.Equal(2, result.Books.Count);
            Assert.Equal("Test Book 1", result.Books[0].Title);
            _getAllBooksQueryMock.Verify(query => query.GetAllAsync(TENANT_ID), Times.Once);
        }

        [Fact]
        public async Task AddBookAsync_ShouldReturnCreatedBookId()
        {
            var request = new AddBookRequest
            {
                Title = "Test Book 1",
                Annotation = "Test annotation 1",
                ArtworkUrl = "http://test1-images.com/img404.png",
                Authors = new List<string>()
                {
                    "Test Author 1"
                },
                Language = "Russian",
            };
            _createBookCommandMock
                .Setup(command => command.CreateAsync(request, TENANT_ID))
                .ReturnsAsync(1);

            var result = await _controller.AddBookAsync(request);

            Assert.Equal(1, result);
            _createBookCommandMock.Verify(command => command.CreateAsync(request, TENANT_ID), Times.Once);
        }

        [Fact]
        public async Task DeleteBook_ShouldCallDeleteAsync()
        {
            var bookId = 1L;

            await _controller.HardDeleteBook(bookId);

            _deleteBookCommandMock.Verify(command => command.DeleteAsync(bookId, TENANT_ID), Times.Once);
        }

        [Fact]
        public async Task UpdateBook_ShouldCallUpdateAsync()
        {
            var updateBookRequest = new UpdateBookRequest()
            {
                Id = 1L,
                Annotation = "Another annotation",
                Title = "Another title",
            };

            await _controller.UpdateBook(updateBookRequest);

            _updateBookCommandMock.Verify(command => command.UpdateAsync(updateBookRequest, TENANT_ID), Times.Once);
        }

    }
}