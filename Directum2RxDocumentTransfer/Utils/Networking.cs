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
        public static string? credentials { get; set; } = string.Empty;

        public static Dictionary<Endpoint, string> endpointMap { get; set; } = new Dictionary<Endpoint, string>();

        public async static Task<string?> SendRequest(string body, Endpoint endpoint)
        {
            Logger.Debug($"SendRequest. Body: {body}. Endpoint: {endpoint}");
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", credentials }
            };
            using (var client = new HttpClient())
            {
                var requestContent = new StringContent(body, Encoding.UTF8, "text/plain");

                if (headers != null)
                    foreach (var header in headers)
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);

                var url = $"{baseUrl}/{endpointMap[endpoint]}";
                Logger.Debug($"SendRequest. Url: {url}");

                var response = await client.PostAsync(url, requestContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    return responseString;
                }
                else
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    Logger.Debug($"SendRequest. Error: {responseString}");
                    return null;
                }
            }
        }
    }
}
