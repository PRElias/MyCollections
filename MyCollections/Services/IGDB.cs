using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MyCollections.Services
{
    public class IGDB
    {
        static HttpClient client = new HttpClient();

        public static async Task<dynamic> GetFromIGDBByCode(string key, string game)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://api-endpoint.igdb.com/games/" + game),
                Method = HttpMethod.Get,
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("user-key", key);

            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var teste2 = response.Content.ReadAsStringAsync().Result;
                return MyCollections.IgdbGame.Igdb.FromJson(teste2);
            }
            else
            {
                return null;
            }
        }

        public static async Task<dynamic> SearchIGDBByNameAndSteamId(string key, string game, string steamId)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://api-endpoint.igdb.com/games/?search=" + game + "&filter[external.steam][eq]=" + steamId),
                Method = HttpMethod.Get,
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("user-key", key);

            HttpResponseMessage response = await client.SendAsync(request);
            var retorno = new RootObject();

            if (response.IsSuccessStatusCode)
            {
                var teste2 = response.Content.ReadAsStringAsync().Result;
                return MyCollections.IgdbIds.IgdbIds.FromJson(teste2);
            }
            else
            {
                return null;
            }

        }

    }

}

public class RootObject
{
    public Response response { get; set; }
}

public class Response
{
    public int id { get; set; }
}