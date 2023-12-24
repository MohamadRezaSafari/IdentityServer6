
//Console.WriteLine("");
//using System.Net.Http.Headers;
//using IdentityModel.Client;

//using var identityServerHttpClient = new HttpClient
//{
//    BaseAddress = new Uri(Environment.GetEnvironmentVariable("AUTHENTICATION__AUTHORITY")!)
//};

//var discoveryDocumentResponse = await identityServerHttpClient.GetDiscoveryDocumentAsync();

//Console.WriteLine(discoveryDocumentResponse.TokenEndpoint);

//var tokenResponse = await identityServerHttpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
//{
//    Address = discoveryDocumentResponse.TokenEndpoint,
//    ClientId = Environment.GetEnvironmentVariable("AUTHENTICATION__CLIENTID")!,
//    ClientSecret = Environment.GetEnvironmentVariable("AUTHENTICATION__CLIENTSECRET")!,
//    Scope = "http://www.example.com/api"
//});

//Console.WriteLine(tokenResponse.AccessToken);

//using var httpClient = new HttpClient
//{
//    DefaultRequestHeaders =
//    {
//        Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken)
//    }
//};

//Thread.Sleep(1000);
//var result = await httpClient.GetStringAsync("http://api:7001/WeatherForecast");
//Thread.Sleep(1000);

//Console.WriteLine(result);


using IdentityModel.Client;
using System.Net.Http.Headers;


//var httpClientHandler = new HttpClientHandler();
//httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
//{
//    return true;
//};

var httpClientHandler = new HttpClientHandler
{
    ClientCertificateOptions = ClientCertificateOption.Manual,
    ServerCertificateCustomValidationCallback =
            (httpRequestMessage, cert, certChain, policyErrors) => true
};

using var identityServerHttpClient = new HttpClient(httpClientHandler)
{
    BaseAddress = new Uri(Environment.GetEnvironmentVariable("AUTHENTICATION__AUTHORITY")!)
};


var discoveryDocumentResponse = await identityServerHttpClient
    .GetDiscoveryDocumentAsync(
        new DiscoveryDocumentRequest
        {
            //Address = "http://xxx.xxx.x.xxx:xxxx",
            Policy =
            {
                RequireHttps = false
            }
        });

Console.WriteLine(discoveryDocumentResponse.TokenEndpoint);


var Address = discoveryDocumentResponse.TokenEndpoint;
var ClientId = Environment.GetEnvironmentVariable("AUTHENTICATION__CLIENTID");
var ClientSecret = Environment.GetEnvironmentVariable("AUTHENTICATION__CLIENTSECRET");
var Scope = "http://www.example.com/api";

var tokenResponse = await identityServerHttpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
    Address = discoveryDocumentResponse.TokenEndpoint,
    ClientId = Environment.GetEnvironmentVariable("AUTHENTICATION__CLIENTID"),
    ClientSecret = Environment.GetEnvironmentVariable("AUTHENTICATION__CLIENTSECRET"),
    Scope = "http://www.example.com/api"
});

Console.WriteLine(tokenResponse.AccessToken);

using var httpClient = new HttpClient()
{
    DefaultRequestHeaders =
    {
        Authorization=new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken)
    }
};

Thread.Sleep(1000);
var result = await httpClient.GetStringAsync("http://api:7001/WeatherForecast");
Thread.Sleep(1000);

Console.WriteLine(result);
