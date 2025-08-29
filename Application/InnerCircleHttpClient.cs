using System.Net.Http.Json;
using System.Text;
using System.Web;
using Core;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Application;

public class InnerCircleHttpClient : IInnerCircleHttpClient
{
    private readonly HttpClient _client;
    private readonly InnerCircleServiceUrls _urls;

    public InnerCircleHttpClient(IOptions<InnerCircleServiceUrls> urls)
    {
        _client = new HttpClient();
        _urls = urls.Value;
    }

    public async Task<Employee> GetEmployeeAsync(string corporateEmail)
    {
        var link = $"{_urls.EmployeesServiceUrl}/internal/get-employee?corporateEmail={corporateEmail}";
        var response = await _client.GetStringAsync(link);

        return JsonConvert.DeserializeObject<Employee>(response);
    }


    public async Task<List<EmployeeById>> GetEmployeesByIdsAsync(List<long> ids)
    {
        var link = $"{_urls.EmployeesServiceUrl}/internal/get-employees-by-ids";

        var response = await _client.PostAsJsonAsync(
            link,
            new
            {
                employeesIds = ids
            }
        );

        var responseContent = await response.Content.ReadAsStringAsync();

        //var responseObject = JsonConvert.DeserializeAnonymousType(
        //    responseContent,
        //    new { employees = new List<Employee>() }
        //);
        Console.WriteLine(JsonConvert.DeserializeObject<List<Employee>>(responseContent));

        return JsonConvert.DeserializeObject<List<EmployeeById>>(responseContent);
        //return responseObject.employees;
    }
}