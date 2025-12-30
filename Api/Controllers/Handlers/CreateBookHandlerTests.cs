using Api.Controllers.Handlers;
using Api.Requests;
using Application.Queries;
using Core.Entities;
using Moq;
using Xunit;

namespace Application.Commands;

public class CreateBookHandlerTests
{
  private const long TENANT_ID = 1;

  private readonly CreateBookHandler _handler;

  public CreateBookHandlerTests()
  {
    var getKnowledgeAreasQueryMock = new Mock<IGetKnowledgeAreasQuery>();

    getKnowledgeAreasQueryMock
      .Setup(x => x.GetByIdsAsync(It.IsAny<List<long>>()))
      .ReturnsAsync(new List<KnowledgeArea>
      {
          new KnowledgeArea {
            Id = 1
          }
      });

    _handler = new CreateBookHandler(getKnowledgeAreasQueryMock.Object, null);
  }

  [Fact]
  public async Task HandleAsyncWithNonExistedKnowledgeAreasId_ShouldThrowException()
  {
    var createBookRequest = new CreateBookRequest
    {
      KnowledgeAreasIds = new List<long> {
        1,
        998,
        999
      },
    };

    var exception = await Assert.ThrowsAsync<ArgumentException>(
      async () => await _handler.HandleAsync(createBookRequest, TENANT_ID)
    );

    Assert.Equal("These knowledge areas IDs were not found: 998, 999", exception.Message);
  }
}
