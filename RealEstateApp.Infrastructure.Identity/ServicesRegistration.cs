using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RealEstateApp.Core.Application.Dtos.User;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Settings;
using RealEstateApp.Infrastructure.Identity.Contexts;
using RealEstateApp.Infrastructure.Identity.Entities;
using RealEstateApp.Infrastructure.Identity.Seeds;
using RealEstateApp.Infrastructure.Identity.Services;
using System.Text;
using System.Text.Json;

namespace RealEstateApp.Infrastructure.Identity
{
    public static class ServicesRegistration
    {
        public static void AddIdentityLayerIocForWebApp(this IServiceCollection services, IConfiguration config)
        {
            GeneralConfiguration(services, config);

            services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = IdentityConstants.ApplicationScheme;
                opt.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                opt.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
            }).AddCookie(IdentityConstants.ApplicationScheme, opt =>
            {
                opt.ExpireTimeSpan = TimeSpan.FromMinutes(180);
                opt.LoginPath = "/Login";
                opt.AccessDeniedPath = "/Login/AccessDenied";
            });

            services.AddScoped<IAccountServiceForWebApp, AccountServiceForWebApp>();
        }

        public static void AddIdentityLayerIocForWebApi(this IServiceCollection services, IConfiguration config)
        {
            GeneralConfiguration(services, config);

            services.Configure<JwtSettings>(config.GetSection("JwtSettings"));

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.RequireHttpsMetadata = false;
                opt.SaveToken = false;
                opt.MapInboundClaims = false;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(2),
                    RoleClaimType = "roles",
                    ValidIssuer = config["JwtSettings:Issuer"],
                    ValidAudience = config["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:SecretKey"] ?? ""))
                };
                opt.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = c =>
                    {
                        c.NoResult();
                        c.Response.StatusCode = 401;
                        c.Response.ContentType = "application/json";
                        var result = JsonSerializer.Serialize(new JwtResponseDto { HasError = true, Error = "El token no es válido" });
                        return c.Response.WriteAsync(result);
                    },
                    OnChallenge = c =>
                    {
                        c.HandleResponse();
                        if (c.Response.HasStarted)
                        {
                            return Task.CompletedTask;
                        }
                        c.Response.StatusCode = 401;
                        c.Response.ContentType = "application/json";
                        var result = JsonSerializer.Serialize(new JwtResponseDto { HasError = true, Error = "No está autorizado" });
                        return c.Response.WriteAsync(result);
                    },
                    OnForbidden = c =>
                    {
                        c.Response.StatusCode = 403;
                        c.Response.ContentType = "application/json";
                        var result = JsonSerializer.Serialize(new JwtResponseDto { HasError = true, Error = "No tiene permisos para acceder a este recurso" });
                        return c.Response.WriteAsync(result);
                    }
                };
            });

            services.AddScoped<IAccountServiceForWebApi, AccountServiceForWebApi>();
        }

        public static async Task RunIdentitySeedAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            await DefaultRoles.SeedAsync(roleManager);
            await DefaultAdminUser.SeedAsync(userManager);
            await DefaultAgentUser.SeedAsync(userManager);
            await DefaultClientUser.SeedAsync(userManager);
            await DefaultDeveloperUser.SeedAsync(userManager);
        }

        private static void GeneralConfiguration(IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("IdentityConnection");
            services.AddDbContext<IdentityContext>(opt =>
                opt.UseSqlServer(connectionString,
                m => m.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName)));

            services.Configure<IdentityOptions>(opt =>
            {
                opt.Password.RequiredLength = 8;
                opt.Password.RequireDigit = true;
                opt.Password.RequireNonAlphanumeric = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;

                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                opt.Lockout.MaxFailedAccessAttempts = 5;

                opt.User.RequireUniqueEmail = true;
                opt.SignIn.RequireConfirmedEmail = true;
            });

            services.AddIdentityCore<AppUser>()
                .AddRoles<IdentityRole>()
                .AddSignInManager()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddTokenProvider<DataProtectorTokenProvider<AppUser>>(TokenOptions.DefaultProvider);

            services.Configure<DataProtectionTokenProviderOptions>(opt =>
            {
                opt.TokenLifespan = TimeSpan.FromHours(12);
            });

            services.AddHttpContextAccessor();
            services.AddScoped<IAgentService, AgentService>();
            services.AddScoped<IUserAdminService, UserAdminService>();
        }
    }
}
