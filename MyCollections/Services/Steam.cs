using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyCollections.Services
{
    public class Steam
    {
        static HttpClient client = new HttpClient();

        public static async Task<RootObject> GetFromSteam(string key, string steamid)
        {
            string path = "IPlayerService/GetOwnedGames/v0001/?key=" + key + "&steamid=" + steamid + "&include_appinfo=1&format=json";
            if (client.BaseAddress == null)
            {
                client.BaseAddress = new Uri("http://api.steampowered.com/");
            }

            HttpResponseMessage response = await client.GetAsync(path);
            var retorno = new RootObject();

            if (response.IsSuccessStatusCode)
            {
                var teste2 = response.Content.ReadAsStringAsync().Result;
                retorno = JsonConvert.DeserializeObject<RootObject>(teste2);
            }

            return retorno;
        }
    }

    public class SteamGame
    {
        public int appid { get; set; }
        public string name { get; set; }
        public int playtime_forever { get; set; }
        public string img_icon_url { get; set; }
        public string img_logo_url { get; set; }
        public bool has_community_visible_stats { get; set; }
        public int? playtime_2weeks { get; set; }
    }

    public class Response
    {
        public int game_count { get; set; }
        public List<SteamGame> games { get; set; }
    }

    public class RootObject
    {
        public Response response { get; set; }
    }
}
