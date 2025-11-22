using System.Net.Http.Json;
using Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

namespace Application;

public class InnerCircleHttpClient : IInnerCircleHttpClient
{
  private readonly HttpClient _client;
  private readonly InnerCircleServiceUrls _urls;
  private readonly AuthenticationOptions _authOptions;
  private readonly IHttpContextAccessor _httpContextAccessor;

  public InnerCircleHttpClient(
    IOptions<InnerCircleServiceUrls> urls,
    IOptions<AuthenticationOptions> authOptions,
    IHttpContextAccessor httpContextAccessor
  )
  {
    _client = new HttpClient();
    _urls = urls.Value;
    _authOptions = authOptions.Value;
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

    var headerName = _authOptions.IsDebugTokenEnabled
      ? "X-DEBUG-TOKEN"
      : "Authorization";

    var token = _httpContextAccessor
      .HttpContext!
      .Request
      .Headers[headerName]
      .ToString();

    _client.DefaultRequestHeaders.Add(headerName, token);

    var response = await _client.PostAsJsonAsync(
      link,
      new
      {
        employeesIds = ids
      }
    );

    var responseContent = await response
      .Content
      .ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
    {
      throw new Exception(
        $"Request to {link} failed. " +
        $"Status: {(int)response.StatusCode} {response.StatusCode}, " +
        $"Body: {responseContent}"
      );
    }

    var responseObject = JsonConvert.DeserializeAnonymousType(
      responseContent,
      new
      {
        employees = new List<EmployeeById>()
      }
    );

    if (responseObject?.employees == null)
    {
      throw new Exception($"Failed to parse employees response: {responseContent}");
    }

    return responseObject.employees;
  }
}
