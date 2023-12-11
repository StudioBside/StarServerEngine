namespace Cs.HttpClient.Detail
{
    using System;
    using System.Diagnostics;
    using Cs.Core;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public sealed class DotNetHost
    {
        public static DotNetHost Instance => Singleton<DotNetHost>.Instance;
        public IHost Host { get; private set; } = null!;

        public void Initialize()
        {
            Debug.Assert(OperatingSystem.IsWindows(), string.Empty);

            if (OperatingSystem.IsWindows())
            {
                var builder = new HostBuilder()
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddHttpClient();
                        services.AddTransient<BasicFactoryHolder>();
                    }).UseConsoleLifetime();

                this.Host = builder.Build();
            }
        }
    }
}
