using Application.Queries;
using Core;

namespace Application.Services;

public interface IBookReadersService
{
  Task<List<EmployeeWhoReadsNow>> GetEmployeesWhoReadNowAsync(List<long> bookCopyIds, long tenantId);
}

public class BookReadersService : IBookReadersService
{
    private readonly IGetBookByIdQuery _getBookByIdQuery;
    private readonly IInnerCircleHttpClient _client;

    public BookReadersService(
      IGetBookByIdQuery getBookByIdQuery,
      IInnerCircleHttpClient client
    )
    {
      _getBookByIdQuery = getBookByIdQuery;
      _client = client;
    }

    public async Task<List<EmployeeWhoReadsNow>> GetEmployeesWhoReadNowAsync(List<long> bookCopyIds, long tenantId)
    {
      var employeesWhoReadNowWithoutFullNames = await _getBookByIdQuery.GetEmployeesWhoReadNowAsync(bookCopyIds, tenantId);

      if (!employeesWhoReadNowWithoutFullNames.Any())
      {
        return new List<EmployeeWhoReadsNow>();
      } 

      var employeeIds = employeesWhoReadNowWithoutFullNames
        .Select(x => x.EmployeeId)
        .ToList();

      var employeesByIds = await _client.GetEmployeesByIdsAsync(employeeIds);
      
      var employeesDict = employeesByIds.ToDictionary(x => x.EmployeeId);

      return employeesWhoReadNowWithoutFullNames
        .Select(x => new EmployeeWhoReadsNow
        {
            EmployeeId = x.EmployeeId,
            FullName = employeesDict[x.EmployeeId].FullName,
            BookCopyId = x.BookCopyId
        })
        .ToList();
    }
}
