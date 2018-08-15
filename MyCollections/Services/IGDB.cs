using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MyCollections.Models;

namespace MyCollections.Services
{
    public class IGDB
    {
        static HttpClient client = new HttpClient();

        public static async Task<dynamic> GetFromIGDB(string key, string game)
        {
            var request = new HttpRequestMessage()
            {
                // RequestUri = new Uri("https://api-endpoint.igdb.com/?search=" + game),
                RequestUri = new Uri("https://api-endpoint.igdb.com/games/" + game),
                Method = HttpMethod.Get,
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("user-key", key);

            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var teste2 = response.Content.ReadAsStringAsync().Result;
                return Igdb.FromJson(teste2);
            }
            else
            {
                return null;
            }
        }
    }
}
