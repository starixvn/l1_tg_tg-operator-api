using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TgOperator.Shared.Configuration
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddAppConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Bind and register MongoDB settings
            services.Configure<MongoDbSettings>(
                configuration.GetSection(nameof(MongoDbSettings)));

            // Bind and register Sentry settings
            services.Configure<SentrySettings>(
                configuration.GetSection(nameof(SentrySettings)));

            // You can add more configuration bindings here
            
            return services;
        }
    }
} 