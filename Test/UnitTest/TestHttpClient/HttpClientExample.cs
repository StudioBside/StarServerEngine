namespace UnitTest.TestHttpClient;

using System.Threading.Tasks;
using Cs.HttpClient;
using Cs.Logging;

[TestClass]
public class HttpClientExample
{
    [TestMethod]
    public async Task 기본_사용법()
    {
        var apiClient = new RestApiClient("https://google.com");
        
        var response = await apiClient.GetStringAsync(string.Empty);
        Log.Info($"response: {response}");
    }
}
