using System;
using System.Buffers.Text;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JustEat.StatsD;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stats.Utils;

namespace Stats
{
    public class Runner
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly Settings _settings;
        private readonly IStatsDPublisher _statsDPublisher;

        public Runner(IHttpClientFactory clientFactory, Settings settings, IStatsDPublisher statsDPublisher)
        {
            _clientFactory = clientFactory;
            _settings = settings;
            _statsDPublisher = statsDPublisher;

//
//            Metrics.Configure(new MetricsConfig
//            {
//                StatsdServerName = _settings.StatsDServer,
//                Prefix = _settings.AppName
//            });
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            var client = _clientFactory.CreateClient();
            var authUrl = _settings.ServerUrl + "/api/auth/signin";
            Console.WriteLine("Sending request to " + authUrl);
            var authResult = await client.PostAsync(authUrl, new JsonContent(new
                {
                    email = _settings.Email,
                    password = _settings.Password
                }), cancellationToken)
                .GetJson<AuthenticateResponse>();

            client.DefaultRequestHeaders.Add("X-Token", authResult.Token);
            client.DefaultRequestHeaders.Add("X-Username", _settings.Username);

            var result = await client.GetAsync(_settings.ServerUrl + "/api/user/memory", cancellationToken).GetJson<GetMemoryResponse>();

            var bytes = Convert.FromBase64String(result.MemoryGzip.Substring(3));
            var input = new MemoryStream(bytes);
            var output = new MemoryStream();
            using (GZipStream decompressionStream = new GZipStream(input, CompressionMode.Decompress))
            {
                decompressionStream.CopyTo(output);
            }

            output.Seek(0, SeekOrigin.Begin);
            var finalMemory = Encoding.UTF8.GetString(output.ToArray());


            JObject stats = JObject.Parse(finalMemory);

            if (!string.IsNullOrEmpty(_settings.MemoryPath))
            {
                stats = (JObject) stats[_settings.MemoryPath];
            }

            Console.WriteLine("Sending metrics...");
            SendMetrics("", stats, 0);
        }

        private void SendMetrics(string path, JObject obj, int level)
        {
            if (level == 10)
            {
                return;
            }

            foreach (var property in obj.Properties())
            {
                var prop = property.Value;
                if (prop.Type == JTokenType.Float || prop.Type == JTokenType.Integer)
                {
                    var d = (double) prop;
                    _statsDPublisher.Gauge(d, path + property.Name);
                }
                else if (prop.Type == JTokenType.Array)
                {
                    var jArray = (JArray) prop;
                    for (var index = 0; index < jArray.Count; index++)
                    {
                        var element = (JObject) jArray[index];
                        SendMetrics(path + property.Name + "." + index + ".", element, level + 1);
                    }
                }
                else if (prop.Type == JTokenType.Object)
                {
                    SendMetrics(path + property.Name + ".", (JObject) prop, level + 1);
                }
            }
        }

        public class AuthenticateResponse
        {
            public string Ok { get; set; }
            public string Token { get; set; }
        }

        public class GetMemoryResponse
        {
            public string Ok { get; set; }

            [JsonProperty("data")]
            public string MemoryGzip { get; set; }
        }


        public class StatsMemory
        {
            [JsonProperty("gclProgress")]
            public decimal GclProgress { get; set; }

            [JsonProperty("gclProgressTotal")]
            public decimal GclProgressTotal { get; set; }

            [JsonProperty("creeps")]
            public decimal Creeps { get; set; }

            [JsonProperty("cpu")]
            public decimal Cpu { get; set; }
        }
    }
}
