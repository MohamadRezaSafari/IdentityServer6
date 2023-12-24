using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Net;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();



builder.Services.AddAuthentication(authenticationOptions =>
{
    authenticationOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    authenticationOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})

    //.AddCookie(cfg => cfg.SlidingExpiration = true)
    //.AddJwtBearer(cfg =>
    //{
    //    cfg.Audience = "http://localhost:4200/";
    //    cfg.Authority = "http://localhost:5000/";
    //    cfg.RequireHttpsMetadata = false;
    //    cfg.SaveToken = true;
    //    cfg.TokenValidationParameters = new TokenValidationParameters
    //    {
    //        ValidateIssuerSigningKey = true,
    //        IssuerSigningKey = GetSignInKey(),
    //        ValidateIssuer = true,
    //        ValidIssuer = GetIssuer(),
    //        ValidateAudience = true,
    //        ValidAudience = GetAudience(),
    //        ValidateLifetime = true,
    //        ClockSkew = TimeSpan.Zero
    //    };
    //    cfg.Configuration = new OpenIdConnectConfiguration();
    //   })
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
        openIdConnectOptions.Scope.Add("http://www/example.com/api");
        openIdConnectOptions.SaveTokens = true;
    })
    ;
builder.Services.AddAuthorization();

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
