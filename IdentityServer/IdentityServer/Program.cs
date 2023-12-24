using System.Reflection;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using IdentityServer.Data;
using IdentityServer.Factories;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using static Duende.IdentityServer.IdentityServerConstants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, dbContextOptionsBuilder) =>
{
    dbContextOptionsBuilder.UseNpgsql(
        serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("Identity"),
        NpgsqlOptionsAction);
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipleFactory>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityServer(options =>
{
    
    options.MutualTls.Enabled = true;
})
    .AddAspNetIdentity<ApplicationUser>()
    .AddConfigurationStore(configurationStoreOptions =>
    {
        configurationStoreOptions.ResolveDbContextOptions = ResolveDbContextOptions;
    })
    .AddOperationalStore(operationalStoreOptions =>
    {
        operationalStoreOptions.ResolveDbContextOptions = ResolveDbContextOptions;
    });

builder.Services.AddRazorPages();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();

app.MapRazorPages();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.MigrateAsync();
    await scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.MigrateAsync();
    await scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.MigrateAsync();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    if (await userManager.FindByNameAsync("thomas.clark") == null)
    {
        await userManager.CreateAsync(
            new ApplicationUser
            {
                UserName = "thomas.clark",
                Email = "thomas.clark@example.com",
                GivenName = "Thomas",
                FamilyName = "Clark"
            }, "Pa55w0rd!");
    }

    var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

    if (!await configurationDbContext.ApiResources.AnyAsync())
    {
        await configurationDbContext.ApiResources.AddAsync(new ApiResource
        {
            Name = "9fc33c2e-dbc1-4d0a-b212-68b9e07b3ba0",
            DisplayName = "API",
            // Scopes = new List<string> { "https://www.example.com/api" }
            Scopes = new List<string> { "http://www.example.com/api" }
        }.ToEntity());


        await configurationDbContext.SaveChangesAsync();
    }

    if (!await configurationDbContext.ApiScopes.AnyAsync())
    {
        await configurationDbContext.ApiScopes.AddAsync(new ApiScope
        {
            // Name = "https://www.example.com/api",
            Name = "http://www.example.com/api",
            DisplayName = "API"
        }.ToEntity());

        await configurationDbContext.SaveChangesAsync();
    }

    if (!await configurationDbContext.Clients.AnyAsync())
    {
        await configurationDbContext.Clients.AddRangeAsync(
            new Client
            {
                ClientId = "b4e758d2-f13d-4a1e-bf38-cc88f4e290e1",
                ClientSecrets = new List<Secret> { new("secret".Sha512()) },
                ClientName = "Console Application",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                // AllowedScopes = new List<string> { "https://www.example.com/api" },
                AllowedScopes = { "http://www.example.com/api" },
                // AllowedCorsOrigins = new List<string> { "https://api:7001" }
                AllowedCorsOrigins = new List<string> { "http://api:7001" }
            }.ToEntity(),
            new Client
            {
                ClientId = "4ecc4153-daf9-4eca-8b60-818a63637a81",
                ClientSecrets = new List<Secret> { new("secret".Sha512()) },
                ClientName = "Web Application",
                AllowedGrantTypes = GrantTypes.Code,
                // AllowedScopes = new List<string> { "openid", "profile", "email", "https://www.example.com/api" },
                AllowedScopes = new List<string> { "openid", "profile", "email", "http://www.example.com/api" },
                // RedirectUris = new List<string> { "https://webapplication:7002/signin-oidc" },
                RedirectUris = { "http://webapplication:7002/signin-oidc" },
                // PostLogoutRedirectUris = new List<string> { "https://webapplication:7002/signout-callback-oidc" }
                PostLogoutRedirectUris = new List<string> { "http://webapplication:7002/signout-callback-oidc" }
            }.ToEntity(),
            new Client
            {
                ClientId = "7e98ad57-540a-4191-b477-03d88b8187e1",
                RequireClientSecret = false,
                ClientName = "Single Page Application",
                AllowedGrantTypes = GrantTypes.Code,
                // AllowedScopes = new List<string> { "openid", "profile", "email", "https://www.example.com/api" },
                AllowedScopes = new List<string> { "openid", "profile", "email", "http://www.example.com/api" },
                AllowedCorsOrigins = new List<string> { "http://singlepageapplication:7003" },
                RedirectUris =
                    new List<string> { "http://singlepageapplication:7003/authentication/login-callback" },
                PostLogoutRedirectUris = new List<string>
                {
                    "http://singlepageapplication:7003/authentication/logout-callback"
                }
            }.ToEntity());

        await configurationDbContext.SaveChangesAsync();
    }

    if (!await configurationDbContext.IdentityResources.AnyAsync())
    {
        await configurationDbContext.IdentityResources.AddRangeAsync(
            new IdentityResources.OpenId().ToEntity(),
            new IdentityResources.Profile().ToEntity(),
            new IdentityResources.Email().ToEntity());

        await configurationDbContext.SaveChangesAsync();
    }
}

app.Run();

void NpgsqlOptionsAction(NpgsqlDbContextOptionsBuilder npgsqlDbContextOptionsBuilder)
{
    npgsqlDbContextOptionsBuilder.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
}

void ResolveDbContextOptions(IServiceProvider serviceProvider, DbContextOptionsBuilder dbContextOptionsBuilder)
{
    dbContextOptionsBuilder.UseNpgsql(
        serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("IdentityServer"),
        NpgsqlOptionsAction);
}



//using System.Reflection;
//using Duende.IdentityServer.EntityFramework.DbContexts;
//using Duende.IdentityServer.EntityFramework.Entities;
//using Duende.IdentityServer.Models;
//using IdentityModel;
//using IdentityServer.Data;
//using IdentityServer.Factories;
//using IdentityServer.Models;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
//using ApiResource = Duende.IdentityServer.EntityFramework.Entities.ApiResource;
//using ApiScope = Duende.IdentityServer.EntityFramework.Entities.ApiScope;
//using Client = Duende.IdentityServer.EntityFramework.Entities.Client;
//using IdentityResource = Duende.IdentityServer.EntityFramework.Entities.IdentityResource;

//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddCors(corsOptions =>
//{
//    corsOptions.AddDefaultPolicy(corsPolicyBuilder =>
//    {
//        corsPolicyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
//    });
//});


//builder.Services
//    .AddDbContext<ApplicationDbContext>((serviceProvider, dbContextOptionsBuilder) =>
//    {
//        dbContextOptionsBuilder.UseNpgsql(
//            serviceProvider.GetRequiredService<IConfiguration>()
//                .GetConnectionString("Identity"), NpgsqlOptionsAction);
//    });


//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//    .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipleFactory>()
//    .AddDefaultTokenProviders()
//    .AddEntityFrameworkStores<ApplicationDbContext>();


//builder.Services.AddIdentityServer()
//    .AddAspNetIdentity<ApplicationUser>()
//    .AddConfigurationStore(configurationStoreOptions =>
//    {
//        configurationStoreOptions.ResolveDbContextOptions = ResolveDbContextOptions;
//    })
//    .AddOperationalStore(options =>
//    {
//        options.ResolveDbContextOptions = ResolveDbContextOptions;
//    });

//builder.Services.AddRazorPages();



//var app = builder.Build();

//app.UseCors();
//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();
//app.UseIdentityServer();
//app.UseAuthorization();
//app.MapRazorPages();


//if (app.Environment.IsDevelopment())
//{
//    using var scope = app.Services.CreateScope();

//    await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.MigrateAsync();
//    await scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.MigrateAsync();
//    await scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.MigrateAsync();

//    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

//    if (await userManager.FindByNameAsync("thomas.clark") is null)
//    {
//        await userManager.CreateAsync(new ApplicationUser
//        {
//            UserName = "thomas.clark",
//            Email = "thomas.clark@gmail.com",
//            GiveName = "Thomas",
//            FamilyName = "Clark"
//        }, "Pass123@#0");
//    }

//    var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

//    if (!await configurationDbContext.ApiResources.AnyAsync())
//    {
//        await configurationDbContext.ApiResources.AddAsync(new ApiResource
//        {
//            Name = "77900020-bd43-435a-81c2-7ddb3b31af76",
//            //Guid.NewGuid().ToString(),
//            DisplayName = "API",
//            Scopes = new List<ApiResourceScope>()
//            {
//                new ApiResourceScope()
//                {
//                    Scope = "https://www.example.com/api"
//                }
//            }
//        });

//        await configurationDbContext.SaveChangesAsync();
//    }

//    if (!await configurationDbContext.ApiScopes.AnyAsync())
//    {
//        await configurationDbContext.ApiScopes.AddAsync(new ApiScope()
//        {
//            Name = "https://www.example.com/api",
//            DisplayName = "API"
//        });

//        await configurationDbContext.SaveChangesAsync();
//    }

//    if (!await configurationDbContext.Clients.AnyAsync())
//    {
//        await configurationDbContext.Clients.AddRangeAsync(new List<Client>()
//        {
//            new Client()
//            {
//                ClientId = "c6903a79-10dc-4138-85ee-6a8fef175fee",
//                //Guid.NewGuid().ToString(),
//                ClientSecrets = new List<ClientSecret>()
//                {
//                    new ClientSecret()
//                    {
//                        Value = "secret".ToSha512()
//                    }
//                },
//                ClientName = "Console Application",
//                AllowedGrantTypes = new List<ClientGrantType>()
//                {
//                    new ClientGrantType()
//                    {
//                        GrantType = nameof(GrantTypes.ClientCredentials)
//                    }
//                },
//                AllowedScopes = new List<ClientScope>()
//                {
//                    new ClientScope()
//                    {
//                        Scope = "https://www.example.com/api"
//                    }
//                },
//                AllowedCorsOrigins = new List<ClientCorsOrigin>()
//                {
//                    new ClientCorsOrigin()
//                    {
//                        Origin = "https://api.7001"
//                    }
//                }
//            },
//            new Client()
//            {
//                ClientId = "f624f078-d325-42ce-844e-48f069da597d",
//                //Guid.NewGuid().ToString(),
//                ClientSecrets = new List<ClientSecret>()
//                {
//                    new ClientSecret()
//                    {
//                        Value = "secret".ToSha512()
//                    }
//                },
//                ClientName = "Web Application",
//                AllowedGrantTypes = new List<ClientGrantType>()
//                {
//                    new ClientGrantType()
//                    {
//                        GrantType = nameof(GrantTypes.Code)
//                    }
//                },
//                AllowedScopes = new List<ClientScope>()
//                {
//                    new ClientScope()
//                    {
//                        Scope = "https://www.example.com/api"
//                    },
//                    new ClientScope()
//                    {
//                        Scope = "openid"
//                    },
//                    new ClientScope()
//                    {
//                        Scope = "profile"
//                    },
//                    new ClientScope()
//                    {
//                        Scope = "email"
//                    }
//                },
//                RedirectUris = new List<ClientRedirectUri>()
//                {
//                    new ClientRedirectUri()
//                    {
//                        RedirectUri = "https://www.webapplication:7002/signin-oidc"
//                    }
//                },
//                PostLogoutRedirectUris = new List<ClientPostLogoutRedirectUri>()
//                {
//                    new ClientPostLogoutRedirectUri()
//                    {
//                        PostLogoutRedirectUri = "https://webapplication:7002/signout-callback-oidc"
//                    }
//                }
//            },
//            new Client()
//            {
//                ClientId = "dd70e6ed-3775-4521-9e22-1a6ccc17303a",
//                //Guid.NewGuid().ToString(),
//                RequireClientSecret = false,
//                ClientName = "Single Page Application",
//                AllowedGrantTypes = new List<ClientGrantType>()
//                {
//                    new ClientGrantType()
//                    {
//                        GrantType = nameof(GrantTypes.Code)
//                    }
//                },
//                AllowedScopes = new List<ClientScope>()
//                {
//                    new ClientScope()
//                    {
//                        Scope = "https://www.example.com/api"
//                    },
//                    new ClientScope()
//                    {
//                        Scope = "openid"
//                    },
//                    new ClientScope()
//                    {
//                        Scope = "profile"
//                    },
//                    new ClientScope()
//                    {
//                        Scope = "email"
//                    }
//                },
//                AllowedCorsOrigins = new List<ClientCorsOrigin>()
//                {
//                    new ClientCorsOrigin()
//                    {
//                        Origin = "http://singlepageapplication:7003"
//                    }
//                },
//                RedirectUris = new List<ClientRedirectUri>()
//                {
//                    new ClientRedirectUri()
//                    {
//                        RedirectUri = "http://singlepageapplication:7003/authentication/login-callback"
//                    }
//                },
//                PostLogoutRedirectUris = new List<ClientPostLogoutRedirectUri>()
//                {
//                    new ClientPostLogoutRedirectUri()
//                    {
//                        PostLogoutRedirectUri = "http://singlepageapplication:7003/authentication/logout-callback"
//                    }
//                }
//            }
//        });

//        await configurationDbContext.SaveChangesAsync();
//    }

//    if (!await configurationDbContext.IdentityResources.AnyAsync())
//    {
//        await configurationDbContext.IdentityResources.AddRangeAsync(new List<IdentityResource>()
//        {
//            new IdentityResource()
//            {
//                Name = nameof(IdentityResources.OpenId)
//            },
//            new IdentityResource()
//            {
//                Name = nameof(IdentityResources.Profile)
//            },
//            new IdentityResource()
//            {
//                Name = nameof(IdentityResources.Email)
//            }
//        });

//        await configurationDbContext.SaveChangesAsync();
//    }
//}


//app.Run();


//void NpgsqlOptionsAction(NpgsqlDbContextOptionsBuilder optionsBuilder)
//{
//    optionsBuilder.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
//}

//void ResolveDbContextOptions(IServiceProvider serviceProvider,
//    DbContextOptionsBuilder dbContextOptionBuilder)
//{
//    dbContextOptionBuilder.UseNpgsql(serviceProvider.GetRequiredService<IConfiguration>()
//            .GetConnectionString("IdentityServer"),
//            NpgsqlOptionsAction);
//}
