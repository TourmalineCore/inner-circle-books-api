
using Api.Requests;
using Api.Responses;
using Application.Commands;
using Application.Queries;
using Core.Entities;

namespace Api.Controllers.Handlers;

public class  CreateBookHandler
{
  private readonly IGetKnowledgeAreasQuery _getKnowledgeAreasQuery;
  private readonly CreateBookCommand _createBookCommand;
    
  public CreateBookHandler(
    IGetKnowledgeAreasQuery getKnowledgeAreasQuery,
    CreateBookCommand createBookCommand
  )
  {
    _getKnowledgeAreasQuery = getKnowledgeAreasQuery;
    _createBookCommand = createBookCommand;
  }

  public async Task<CreateBookResponse> HandleAsync( CreateBookRequest createBookRequest, long tenantId )
  {
    var knowledgeAreas = await _getKnowledgeAreasQuery.GetByIdsAsync(createBookRequest.KnowledgeAreasIds);

    var nonExistentKnowledgeAreasIds = createBookRequest
      .KnowledgeAreasIds
      .Except(knowledgeAreas.Select(x => x.Id));

    if (nonExistentKnowledgeAreasIds.Any())
    {
      throw new ArgumentException($"These knowledge areas IDs were not found: {string.Join(", ", nonExistentKnowledgeAreasIds)}");
    }

    var authors = createBookRequest
      .Authors
      .Select(author => new Author
      {
        FullName = author.FullName,
      })
      .ToList();

    var createBookCommandParams = new CreateBookCommandParams
    {
      Title = createBookRequest.Title,
      Annotation = createBookRequest.Annotation,
      Authors = authors,
      Language = (Language)Enum.Parse(typeof(Language), createBookRequest.Language),
      KnowledgeAreas = knowledgeAreas,
      CoverUrl = createBookRequest.CoverUrl,
      CountOfCopies = createBookRequest.CountOfCopies,
    };

    var newBookId = await _createBookCommand.CreateAsync(createBookCommandParams, tenantId);

    return new CreateBookResponse()
    {
      NewBookId = newBookId
    };
  }
}
