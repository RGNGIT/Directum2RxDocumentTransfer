﻿using Newtonsoft.Json;
using RestSharp;

namespace Directum2RxDocumentTransfer.Utils
{
    public static class Networking
    {
        public enum Endpoint
        {
            Visas,
            Remarks
        }

        public static string? baseUrl { get; set; } = string.Empty;
        public static string? credentials { get; set; } = string.Empty;

        public static Dictionary<Endpoint, string> endpointMap { get; set; } = new Dictionary<Endpoint, string>();

        public async static Task<string?> SendRequest(object body, Endpoint endpoint)
        {
            Logger.Debug($"SendRequest. Body: {JsonConvert.SerializeObject(body)}. Endpoint: {endpoint}");
            Logger.Debug($"SendRequest. Url: {baseUrl}/{endpointMap[endpoint]}");

            var client = new RestClient(baseUrl);
            var request = new RestRequest($"/{endpointMap[endpoint]}", Method.Post);

            var jsonBody = JsonConvert.SerializeObject(body);
            request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);

            request.AddHeader("Authorization", credentials);
            request.AddHeader("Content-Type", "application/json");

            try
            {
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                    return response.Content;
                else
                {
                    Logger.Debug($"SendRequest. Error: {response.Content}");
                    return "Network error";
                }
            }
            catch (System.Exception ex)
            {
                Logger.Debug($"SendRequest. Exception: {ex.Message}");
                return "Exception";
            }
        }
    }
}
