using Microsoft.AspNetCore.Mvc;
using MyCollections.Models;
using MyCollections.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyCollections.Services
{
    public class Steam
    {
        private static HttpClient _client = new HttpClient();
        private static string _steamKey;
        private static string _steamId;

        public Steam(string key, string id)
        {
            _steamKey = key;
            _steamId = id;
        }

        public static async Task<RootObject> GetFromSteam()
        {
            string path = "IPlayerService/GetOwnedGames/v0001/?key=" + _steamKey + "&steamid=" + _steamId + "&include_appinfo=1&format=json";
            if (_client.BaseAddress == null)
            {
                _client.BaseAddress = new Uri("http://api.steampowered.com/");
            }

            HttpResponseMessage response = await _client.GetAsync(path);
            var retorno = new RootObject();

            if (response.IsSuccessStatusCode)
            {
                var teste2 = await response.Content.ReadAsStringAsync();
                retorno = JsonConvert.DeserializeObject<RootObject>(teste2);
            }

            return retorno;
        }

        public static async Task<string> SearchGameByName(string gameName)
        {
            string encodedGameName = System.Net.WebUtility.UrlEncode(gameName);
            string searchUrl = $"https://store.steampowered.com/api/storesearch/?term={encodedGameName}&cc=us&l=en&v=1";
            HttpResponseMessage response = await _client.GetAsync(searchUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var searchResults = JsonConvert.DeserializeObject<SearchResults>(jsonResponse);

                if (searchResults.items != null && searchResults.items.Count > 0)
                {
                    return searchResults.items[0].id.ToString();
                }
            }

            return null;
        }
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

    public class SearchResults
    {
        public List<SearchItem> items { get; set; }
    }

    public class SearchItem
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
