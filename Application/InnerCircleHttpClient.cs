using System.Net.Http.Json;
using Core;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace Application;

public class InnerCircleHttpClient : IInnerCircleHttpClient
{
    private readonly HttpClient _client;
    private readonly InnerCircleServiceUrls _urls;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public InnerCircleHttpClient(
        IOptions<InnerCircleServiceUrls> urls,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _client = new HttpClient();
        _urls = urls.Value;
        _httpContextAccessor = httpContextAccessor;
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

        var authHeader = _httpContextAccessor
            .HttpContext?
            .Request
            .Headers["Authorization"]
            .ToString();

        if (!string.IsNullOrEmpty(authHeader))
        {
            _client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(authHeader);
        }

        var response = await _client.PostAsJsonAsync(
            link,
            new
            {
                employeesIds = ids
            }
        );

        var responseContent = await response.Content.ReadAsStringAsync();

        var responseObject = JsonConvert.DeserializeAnonymousType(
            responseContent,
            new
            {
                employees = new List<EmployeeById>()
            }
        );

        return responseObject.employees;
    }
}