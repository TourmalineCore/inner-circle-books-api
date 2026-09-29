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
      List<AvailabilityStatus> availabilityStatuses 
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
          .Select(a => new AuthorResponse()
          {
            FullName = a.FullName
          })
          .ToList(),
        Language = book.Language.ToString(),
        KnowledgeAreas = book
          .KnowledgeAreas
          .Select(k => new KnowledgeAreaItem
          {
            Id = k.Id,
            Name = k.Name
          })
          .ToList(),
        BookCopiesIds = bookCopiesIds,
        EmployeesWhoReadNow = employeesWhoReadNow,
        AvailabilityStatuses = availabilityStatuses
          .Select(s => s.ToString())
          .ToList()
      };
    }
}
