using System.Net.Http;
using System.Threading.Tasks;

public class NetworkHelper
{
    public static async Task<bool> IsOnline()
    {
        try
        {
            using var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(3)
            };

            using var response = await httpClient.GetAsync("https://www.google.com");

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
