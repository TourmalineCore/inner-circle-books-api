using Api.Responses;
using Core;
using Core.Entities;

namespace Api.Mappers;

public static class SingleBookResponseMapper
{
    public static SingleBookResponse Map(
      Book book,
      List<long> bookCopiesIds,
      List<EmployeeWhoReadsNow> employeesWhoReadNow,
      List<BookAvailabilityStatus> bookAvailabilityStatuses 
    )
    {
      return new SingleBookResponse
      {
        Id = book.Id,
        Title = book.Title,
        Annotation = book.Annotation,
        CoverUrl = book.CoverUrl,
        Authors = book
          .Authors
          .Select(x => new AuthorResponse()
          {
            FullName = x.FullName
          })
          .ToList(),
        Language = book.Language.ToString(),
        KnowledgeAreas = book
          .KnowledgeAreas
          .Select(x => new KnowledgeAreaItem
          {
            Id = x.Id,
            Name = x.Name
          })
          .ToList(),
        BookCopiesIds = bookCopiesIds,
        EmployeesWhoReadNow = employeesWhoReadNow,
        AvailabilityStatuses = bookAvailabilityStatuses
          .Select(x => x.ToString())
          .ToList()
      };
    }
}
