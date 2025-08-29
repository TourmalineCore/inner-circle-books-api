using Core;

namespace Application;

public interface IInnerCircleHttpClient
{
    Task<Employee> GetEmployeeAsync(string corporateEmail);
    Task<List<Employee>> GetEmployeesByIdsAsync(List<long> ids);
}