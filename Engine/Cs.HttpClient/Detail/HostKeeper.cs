namespace Cs.HttpClient.Detail
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Sockets;
    using Cs.Core;
    using DnsRoundRobin;
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
                    services.AddHttpClient().ConfigureHttpClientDefaults(builder =>
                    {
                        builder.ConfigurePrimaryHttpMessageHandler(() =>
                        {
                            return new SocketsHttpHandler()
                            {
                                //ServicePointManager.DefaultConnectionLimit = int.MaxValue;
                                MaxConnectionsPerServer = int.MaxValue,

                                //ServicePointManager.EnableDnsRoundRobin = true;
                                // https://learn.microsoft.com/ko-kr/dotnet/fundamentals/networking/http/httpclient-migrate-from-httpwebrequest#example-enable-dns-round-robin
                                ConnectCallback = async (context, cancellation) =>
                                {
                                    Socket socket = await DnsRoundRobinConnector.Shared.ConnectAsync(context.DnsEndPoint, cancellation);

                                    // 닷넷 코어에서는 기본 false입니다.
                                    //ServicePointManager.UseNagleAlgorithm = false;
                                    socket.NoDelay = true;
                                    //ServicePointManager.ReusePort = true;
                                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseUnicastPort, true);

                                    // Or you can create and use your custom DnsRoundRobinConnector instance
                                    // Socket socket = await s_roundRobinConnector.ConnectAsync(context.DnsEndPoint, cancellation);
                                    return new NetworkStream(socket, ownsSocket: true);
                                },
                            };
                        });

                        builder.ConfigureHttpClient(client =>
                        {
                            // 닷넷 코어에서는 기본 false입니다.
                            //ServicePointManager.Expect100Continue = false;
                            client.DefaultRequestHeaders.ExpectContinue = false;
                        });
                    });

                    services.AddTransient<BasicClientCreator>();
                }).UseConsoleLifetime();

            this.Host = builder.Build();
        }
    }
}
