using ApplicationCore.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ApplicationCore;

public static class ApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddApplicationCore(this IHostApplicationBuilder builder)
    {
        builder
            .Services.AddScoped<ProductService>()
            .AddScoped<UserService>()
            .AddScoped<StoredRequestsService>()
            .AddHostedService<RepositoryFreshUpService>();

        var config = builder.Configuration;
        builder
            .Services.AddOptions<RepositoryFreshUpServiceOptions>()
            .Bind(config.GetSection("Delays:DbFreshUpHrs"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        return builder;
    }
}
