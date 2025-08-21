using Core;

namespace Application;

public interface IInnerCircleHttpClient
{
    Task<Employee> GetEmployeeAsync(string corporateEmail);
}