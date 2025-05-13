using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ABI.System;
using Microsoft.VisualBasic.Logging;

namespace Directum2RxDocumentTransfer.Utils
{
    public static class Networking
    {
        public enum Endpoint
        {
            Visas
        }

        public static string? baseUrl { get; set; } = string.Empty;
        public static string? visasEndpoint { get; set; } = string.Empty;
        public static string? credentials { get; set; } = string.Empty;

        public static Dictionary<Endpoint, string> endpointMap { get; set; } = new Dictionary<Endpoint, string>()
        {
            { Endpoint.Visas, visasEndpoint }
        };

        public async static Task<string?> SendRequest(string body, Endpoint endpoint)
        {
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", credentials }
            };
            using (var client = new HttpClient())
            {
                var requestContent = new StringContent(body, Encoding.UTF8, "application/json");

                if (headers != null)
                    foreach (var header in headers)
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);

                var response = await client.PostAsync($"{baseUrl}/{endpointMap[endpoint]}", requestContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    return responseString;
                }
                else
                    return null;
            }
        }
    }
}
