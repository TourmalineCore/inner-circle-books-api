using System.Security.Claims;
using Api.Responses;
using Application.Commands.Contracts;
using Application.Queries.Contracts;
using Application.Requests;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Api.Controllers;

public class BooksControllerTests
{
    private const long TENANT_ID = 1;
    private readonly BooksController _controller;
    private readonly Mock<ICreateBookCommand> _createBookCommandMock;
    private readonly Mock<IDeleteBookCommand> _deleteBookCommandMock;
    private readonly Mock<IGetAllBooksQuery> _getAllBooksQueryMock;
    private readonly Mock<IGetBookByIdQuery> _getBookByIdQueryMock;
    private readonly Mock<ISoftDeleteBookCommand> _softDeleteBookCommandMock;
    private readonly Mock<IUpdateBookCommand> _updateBookCommandMock;

    public BooksControllerTests()
    {
        _getAllBooksQueryMock = new Mock<IGetAllBooksQuery>();
        _getBookByIdQueryMock = new Mock<IGetBookByIdQuery>();
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
            _getBookByIdQueryMock.Object,
            _createBookCommandMock.Object,
            _updateBookCommandMock.Object,
            _deleteBookCommandMock.Object,
            _softDeleteBookCommandMock.Object
        )
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            }
        };
    }

    [Fact]
    public async Task GetAllBooksAsync_ShouldReturnBooksResponse()
    {
        var books = new List<Book>
        {
            new()
            {
                Id = 1,
                TenantId = TENANT_ID,
                Title = "Test Book 1",
                Annotation = "Test annotation 1",
                Authors = new List<Author>()
                {
                    new Author
                    {
                        FullName = "Test Author"
                    }
                },
                Language = Language.en,
                BookCoverUrl = "http://test1-images.com/img404.png"
            },
            new()
            {
                Id = 2,
                TenantId = TENANT_ID,
                Title = "Test Book 2",
                Annotation = "Test annotation 2",
                Authors = new List<Author>()
                {
                    new Author()
                    {
                        FullName = "Test Author 2"
                    }
                },
                Language = Language.en,
                BookCoverUrl = "http://test2-images.com/img404.png"
            }
        };
        _getAllBooksQueryMock
            .Setup(query => query.GetAllAsync(TENANT_ID))
            .ReturnsAsync(books);

        var result = await _controller.GetAllBooksAsync();

        Assert.IsType<BooksListResponse>(result);
        Assert.Equal(2, result.Books.Count);
        Assert.Equal("Test Book 1", result.Books[0].Title);
        _getAllBooksQueryMock.Verify(query => query.GetAllAsync(TENANT_ID), Times.Once);
    }

    [Fact]
    public async Task GetBookByIdAsync_ShouldReturnCorrectBook()
    {
        var books = new List<Book>
        {
            new()
            {
                Id = 1,
                TenantId = TENANT_ID,
                Title = "Test Book 1",
                Annotation = "Test annotation 1",
                Authors = new List<Author>()
                {
                    new Author()
                    {
                        FullName = "Test Author"
                    }
                },
                Language = Language.en,
                BookCoverUrl = "http://test1-images.com/img404.png"
            },
            new()
            {
                Id = 2,
                TenantId = TENANT_ID,
                Title = "Test Book 2",
                Annotation = "Test annotation 2",
                Authors = new List<Author>()
                {
                    new Author()
                    {
                        FullName = "Test Author 2"
                    }
                },
                Language = Language.en,
                BookCoverUrl = "http://test2-images.com/img404.png"
            }
        };
        _getBookByIdQueryMock
            .Setup(query => query.GetByIdAsync(1, TENANT_ID))
            .ReturnsAsync(books.Where(x => x.Id == 1).Single);

        var result = await _controller.GetBookByIdAsync(1);

        Assert.Equal("Test Book 1", result.Title);
    }

    [Fact]
    public async Task CreateBookAsync_ShouldReturnCreatedBookId()
    {
        var request = new CreateBookRequest
        {
            Title = "Test Book 1",
            Annotation = "Test annotation 1",
            Authors = new List<AuthorModel>()
            {
                new AuthorModel()
                {
                    FullName = "Test Author"
                }
            },
            BookCoverUrl = "http://test1-images.com/img404.png",
            Language = "ru"
        };
        _createBookCommandMock
            .Setup(command => command.CreateAsync(request, TENANT_ID))
            .ReturnsAsync(1);

        var result = await _controller.CreateBookAsync(request);

        Assert.Equal(1, result.NewBookId);
        _createBookCommandMock.Verify(command => command.CreateAsync(request, TENANT_ID), Times.Once);
    }

    [Fact]
    public async Task DeleteBook_ShouldCallDeleteAsync()
    {
        var bookId = 1;

        await _controller.HardDeleteBook(bookId);

        _deleteBookCommandMock.Verify(command => command.DeleteAsync(bookId, TENANT_ID), Times.Once);
    }

    [Fact]
    public async Task UpdateBook_ShouldCallUpdateAsync()
    {
        var updateBookRequest = new UpdateBookRequest
        {
            Annotation = "Another annotation",
            Title = "Another title",
            Authors = new List<AuthorModel>()
            {
                new AuthorModel()
                {
                    FullName = "Updated Author"
                }
            },
            Language = "en"
        };

        await _controller.UpdateBook(1, updateBookRequest);

        _updateBookCommandMock.Verify(command => command.UpdateAsync(1, updateBookRequest, TENANT_ID), Times.Once);
    }
}