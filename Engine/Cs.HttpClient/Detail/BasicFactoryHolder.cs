namespace Cs.HttpClient.Detail
{
    using System.Net.Http;

    internal sealed class BasicFactoryHolder
    {
        private readonly IHttpClientFactory clientFactory;

        public BasicFactoryHolder(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public HttpClient CreateClient()
        {
            return this.clientFactory.CreateClient();
        }
    }
}
