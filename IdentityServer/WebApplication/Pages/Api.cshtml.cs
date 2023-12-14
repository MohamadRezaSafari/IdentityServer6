using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace WebApplication.Pages
{
    [Authorize]
    public class ApiModel : PageModel
    {
        public string? Data { get; set; }
        private IHttpClientFactory HttpClientFactory { get; set; }

        public ApiModel(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory;
        }

        public void OnGetAsync()
        {
            using var httpClient = HttpClientFactory.CreateClient();

            Task.Run(async () =>
            {
                httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", await HttpContext.GetTokenAsync("access_token"));
                Data = await httpClient.GetStringAsync("https://api:7001/WeatherForecast");
            });
        }
    }
}
