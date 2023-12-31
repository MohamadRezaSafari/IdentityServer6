using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(jwtBearerOptions =>
    {
        jwtBearerOptions.RequireHttpsMetadata = false;
        jwtBearerOptions.Authority = builder.Configuration["Authentication:Authority"];
        jwtBearerOptions.Audience = builder.Configuration["Authentication:Audience"];

        jwtBearerOptions.TokenValidationParameters.ValidateAudience = true;
        jwtBearerOptions.TokenValidationParameters.ValidateIssuer = true;
        jwtBearerOptions.TokenValidationParameters.ValidateIssuerSigningKey = true;
    });

builder.Services.AddAuthorization(authorizationOptions =>
{
    authorizationOptions.AddPolicy("ApiScope", authorizationPolicyBuilder =>
    {
        authorizationPolicyBuilder
            .RequireAuthenticatedUser()
            .RequireClaim("scope", "http://www.example.com/api");
        //.RequireClaim("scope", "https://www.example.com/api");
    });
});

builder.Services.AddControllers();

builder.Services.AddCors(corsOptions =>
{
    corsOptions.AddDefaultPolicy(corsPolicyBuilder =>
    {
        corsPolicyBuilder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(swaggerGenOptions =>
{
    swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

    swaggerGenOptions.AddSecurityDefinition("oauth2",
        new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                ClientCredentials = new OpenApiOAuthFlow
                {
                    TokenUrl =
                        new Uri($"{builder.Configuration["Authentication:Authority"]}/connect/token"),
                    //Scopes = { { "https://www.example.com/api", "API" } }
                    Scopes = { { "http://www.example.com/api", "API" } }
                }
            }
        });

    swaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            },
            //new List<string> { "https://www.example.com/api" }
            new List<string> { "http://www.example.com/api" }
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(options =>
    options
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();



//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.OpenApi.Models;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(jwtBearerOptions =>
//    {
//        //jwtBearerOptions.TokenValidationParameters.ValidateAudience = false;

//        jwtBearerOptions.Authority = builder.Configuration["Authentication:Authority"];
//        jwtBearerOptions.Audience = builder.Configuration["Authentication:Audience"];

//        jwtBearerOptions.TokenValidationParameters.ValidateAudience = true;
//        jwtBearerOptions.TokenValidationParameters.ValidateIssuer = true;
//        jwtBearerOptions.TokenValidationParameters.ValidateIssuerSigningKey = true;
//    });

//builder.Services.AddAuthorization(authorizationOptions =>
//{
//    authorizationOptions.AddPolicy("ApiScope", authorizationPolicyBuilder =>
//    {
//        authorizationPolicyBuilder
//            .RequireAuthenticatedUser()
//            .RequireClaim("scope", "https://www.example.com/api");
//    });
//});

//builder.Services.AddControllers();

//builder.Services.AddCors(corsOptions =>
//{
//    corsOptions.AddDefaultPolicy(corsPolicyBuilder =>
//    {
//        corsPolicyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
//    });
//});


//builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen(swaggerGenOptions =>
//{
//    swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo() { Title = "API", Version = "v1" });

//    swaggerGenOptions.AddSecurityDefinition("oauth2",
//        new OpenApiSecurityScheme
//        {
//            Type = SecuritySchemeType.OAuth2,
//            Flows = new OpenApiOAuthFlows()
//            {
//                ClientCredentials = new OpenApiOAuthFlow
//                {
//                    TokenUrl = new Uri($"{builder.Configuration["Authentication:Authority"]}/connect/token"),
//                    Scopes = { { "https://www.example.com/api ", "API" } }
//                }
//            }
//        });

//    swaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement()
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference =  new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "oauth2"
//                }
//            },
//            new List<string>
//            {
//                "https://www.example.com/api"
//            }
//        }
//    });
//});

//var app = builder.Build();



//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
//app.UseCors();
//app.UseAuthentication();
//app.UseAuthorization();


//app.MapControllers();

//app.Run();
