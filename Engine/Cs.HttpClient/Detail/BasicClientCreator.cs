namespace Cs.HttpClient.Detail
{
    using System.Net.Http;

    internal sealed class BasicClientCreator
    {
        private readonly IHttpClientFactory clientFactory;

        public BasicClientCreator(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public HttpClient Create()
        {
            return this.clientFactory.CreateClient();
        }
    }
}
