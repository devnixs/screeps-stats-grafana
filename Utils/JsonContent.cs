using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Stats.Utils
{
    public class JsonContent : StringContent
    {
        public JsonContent(object obj) :
            base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
        {
        }
    }

    public static class HttpUtils
    {
        public static async Task<T> GetJson<T>(this HttpContent content)
        {
            var str = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(str);
        }

        public static async Task<T> GetJson<T>(this Task<HttpResponseMessage> responseTask)
        {
            var response = await responseTask;
            response.EnsureSuccessStatusCode();
            var str = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}
