using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Quay
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.AddRazorPages();

            services.AddMemoryCache();

            services.Configure<IpRateLimitOptions>(Configuration.GetSection("RateLimits"));
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("RateLimitPolicies"));

            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            services.AddHttpContextAccessor();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseIpRateLimiting();

            app.UseHsts();
            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapFallbackToPage("", "/Index");
                endpoints.MapFallbackToPage("{**url}", "/NotFound");
            });
        }
    }
}
