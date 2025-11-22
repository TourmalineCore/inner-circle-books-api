using Core;

namespace Application;

public interface IInnerCircleHttpClient
{
  Task<Employee> GetEmployeeAsync(string corporateEmail);
  Task<List<EmployeeById>> GetEmployeesByIdsAsync(List<long> ids);
}
