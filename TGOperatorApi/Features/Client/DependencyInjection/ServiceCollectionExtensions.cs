using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTgOperatorClient(
        this IServiceCollection services,
        Action<TgOperatorClientOptions, IServiceProvider> configure)
        => services
        .AddOptions<TgOperatorClientOptions>()
        .Configure(configure)
        .Services
        .AddHttpClient<ITgOperatorClient, TgOperatorClient>()
        .ConfigureHttpClient((sp, httpClient) =>
        {
            var options = sp.GetRequiredService<IOptions<TgOperatorClientOptions>>().Value;
            httpClient.BaseAddress = options.BaseUrl;
        })
        .Services;
}
