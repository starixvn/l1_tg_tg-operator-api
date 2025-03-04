using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TgOperator.Core.Features.Wallet;

public static class WalletMiddlewareExtensions
{
    public static IServiceCollection AddWalletFeature(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<WalletApiSettings>(
            configuration.GetSection(nameof(WalletApiSettings)));

        services.AddHttpClient<IWalletService, WalletService>();

        return services;
    }
} 