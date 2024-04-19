namespace Cs.HttpClient.Detail
{
    using Cs.Core;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public sealed class HostKeeper
    {
        public static HostKeeper Instance => Singleton<HostKeeper>.Instance;
        public IHost Host { get; private set; } = null!;

        public void Initialize()
        {
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHttpClient();
                    services.AddTransient<BasicClientCreator>();
                }).UseConsoleLifetime();

            this.Host = builder.Build();
        }
    }
}
