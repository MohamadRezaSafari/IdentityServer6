using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SinglePageApplication;
using SinglePageApplication.Handlers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddHttpClient("Api", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://api:7001");
}).AddHttpMessageHandler<ApiAuthorizationMessageHandler>();

builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority = builder.Configuration["Authentication:Authority"];
    options.ProviderOptions.ClientId = builder.Configuration["Authentication:ClientId"];
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.DefaultScopes.Add("https://www.example.com/api");
});

builder.Services.AddScoped<ApiAuthorizationMessageHandler>();

await builder.Build().RunAsync();
