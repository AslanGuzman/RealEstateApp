using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Settings;
using RealEstateApp.Infrastructure.Shared.Services;

namespace RealEstateApp.Infrastructure.Shared
{
    public static class ServicesRegistration
    {
        public static void AddSharedLayerIoc(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<MailSettings>(config.GetSection("MailSettings"));

            services.AddScoped<IEmailService, EmailService>();
        }
    }
}
