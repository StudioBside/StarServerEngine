namespace Cs.Cli;

using Cs.Cli.Detail;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExt
{
    public static IServiceCollection AddCsDotCliServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<IHomePathConfig, HomePathConfig>();
    }
}
