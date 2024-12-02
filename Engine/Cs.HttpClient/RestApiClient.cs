namespace Cs.HttpClient
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;

    using System.Threading.Tasks;
    using Cs.HttpClient.Detail;
    using Cs.Logging;
    using Microsoft.Extensions.DependencyInjection;

    public sealed class RestApiClient
    {
        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);
        
        private string authorization = string.Empty;

        static RestApiClient()
        {
            HostKeeper.Instance.Initialize();
        }

        public RestApiClient(string uriString)
        {
            this.Uri = new Uri(uriString);
        }

        public Uri Uri { get; }
        
        public void SetBasicAutohrization(string username, string password)
        {
            var authInfo = $"{username}:{password}";
            var authInfoEnc = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(authInfo));
            this.authorization = $"Basic {authInfoEnc}";
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            try
            {
                using var client = this.CreateClient();
                return await client.SendAsync(request); // 예외를 잡기 위해 여기서 await 필수
            }
            catch (Exception ex)
            {
                Log.Error($"[HttpClientPool] {ex.Message}");
                return new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound };
            }
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            try
            {
                using var client = this.CreateClient();
                return await client.PostAsync(requestUri, content); // 예외를 잡기 위해 여기서 await 필수
            }
            catch (Exception ex)
            {
                Log.Error($"[HttpClientPool] {ex.Message}");
                return new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound };
            }
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            return this.SendAsync(request);
        }

        public async Task<string> GetStringAsync(string requestUri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await this.SendAsync(request);
            if (response == null)
            {
                return string.Empty;
            }

            if (response.IsSuccessStatusCode == false)
            {
                return string.Empty;
            }

            return await response.Content.ReadAsStringAsync();
        }

        //// -------------------------------------------------------------------

        private HttpClient CreateClient()
        {
            var basicCreator = HostKeeper.Instance.Host.Services.GetRequiredService<BasicClientCreator>();
            var client = basicCreator.Create();
            client.BaseAddress = this.Uri;
            client.Timeout = DefaultTimeout;
            if (string.IsNullOrEmpty(this.authorization) == false)
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", this.authorization);
            }

            return client;
        }
    }
}
