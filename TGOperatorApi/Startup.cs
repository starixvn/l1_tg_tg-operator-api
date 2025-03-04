using ThirdPartyService.Extensions;

namespace ThirdPartyService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddControllers();
            services.AddWebProxy(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/health", async context =>
                {
                    await context.Response.WriteAsync($"{configuration.GetValue<string>("EnvironmentName")}-{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}");
                });

                endpoints.MapControllers();
            });
        }
    }
}