using TgOperator.Core.Features.Player;

namespace ThirdPartyService.Extensions
{
    public static class ServiceCollectionExtension
    {
        
        public static void AddWebProxy(this IServiceCollection services, IConfiguration configuration)
        {
            if (!string.IsNullOrWhiteSpace(configuration.GetValue<string>("ProxyHost")))
            {
                VendorHelperBase.proxyHost = configuration.GetValue<string>("ProxyHost");
                VendorHelperBase.proxyPort = configuration.GetValue<int>("ProxyPort");
            }
        }
    }
}