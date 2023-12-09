

using IdentityModel.Client;
using System.Net.Http.Headers;

using var identityServerHttpClient = new HttpClient
{
    BaseAddress = new Uri(Environment.GetEnvironmentVariable("AUTHENTICATION_AUTHORITY")!)
};

var discoveryDocumentResponse = await identityServerHttpClient.GetDiscoveryDocumentAsync();

Console.WriteLine(discoveryDocumentResponse.TokenEndpoint);

var tokenResponse = await identityServerHttpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
    Address = discoveryDocumentResponse.TokenEndpoint,
    ClientId = Environment.GetEnvironmentVariable("AUTHENTICATION_CLIENTID"),
    ClientSecret = Environment.GetEnvironmentVariable("AUTHENTICATION_CLIENTSECRET"),
    Scope = "https://www.example.com/api"
});

Console.WriteLine(tokenResponse.AccessToken);

using var httpClient = new HttpClient()
{
    DefaultRequestHeaders =
    {
        Authorization=new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken)
    }
};

Console.WriteLine(await httpClient.GetStringAsync("https://api:7001/weatherForcast"));
