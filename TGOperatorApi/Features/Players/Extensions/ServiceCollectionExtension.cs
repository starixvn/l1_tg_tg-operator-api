using VendorCore;

namespace ThirdPartyService.Extensions
{
    public static class ServiceCollectionExtension
    {
        
        public static void AddWebProxy(this IServiceCollection services, IConfiguration configuration)
        {
            if (!string.IsNullOrWhiteSpace(configuration.GetValue<string>("ProxyHost")))
            {
                ThirdPartyHelperBase.proxyHost = configuration.GetValue<string>("ProxyHost");
                ThirdPartyHelperBase.proxyPort = configuration.GetValue<int>("ProxyPort");
            }
        }
    }
}