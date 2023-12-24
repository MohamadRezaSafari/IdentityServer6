using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Net;
using Microsoft.Extensions.Options;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();




builder.Services.AddAuthentication(authenticationOptions =>
    {
        authenticationOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        authenticationOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddOpenIdConnect(openIdConnectOptions =>
    {
        openIdConnectOptions.RequireHttpsMetadata = false;
        openIdConnectOptions.Authority = builder.Configuration["Authentication:Authority"];
        openIdConnectOptions.ClientId = builder.Configuration["Authentication:ClientId"];
        openIdConnectOptions.ClientSecret = builder.Configuration["Authentication:ClientSecret"];
        openIdConnectOptions.GetClaimsFromUserInfoEndpoint = true;
        openIdConnectOptions.ResponseType = "code";
        //openIdConnectOptions.Scope.Add("https://www/example.com/api");
        openIdConnectOptions.Scope.Add("http://www.example.com/api");
        openIdConnectOptions.SaveTokens = true;
        openIdConnectOptions.CorrelationCookie = new CookieBuilder
        {
            // HttpOnly = false,
            SameSite = SameSiteMode.None
            // SecurePolicy = CookieSecurePolicy.None

            // SameSite = SameSiteMode.None,
            // HttpOnly = true,
            // SecurePolicy = CookieSecurePolicy.Always,
            // Expiration = DateTime.UtcNow + NonceLifetime
        };
        openIdConnectOptions.ProtocolValidator = new OpenIdConnectProtocolValidator()
        {
            RequireNonce = false,
            RequireState = false
        };

        //openIdConnectOptions.NonceCookie = new CookieBuilder()
        //{
        //    HttpOnly = false,
        //    SameSite = SameSiteMode.None,
        //    SecurePolicy = CookieSecurePolicy.None,
        //    Expiration = TimeSpan.FromMinutes(10)
        //};
    });
builder.Services.AddAuthorization();


builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});



builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();