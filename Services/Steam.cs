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
        private static MyCollectionsRepository _db;

        public Steam([FromServices] MyCollectionsRepository db)
        {
            _db = db;
            _steamKey = _db.GetAll().steamKey;
            _steamId = _db.GetAll().steamId;
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
                var teste2 = response.Content.ReadAsStringAsync().Result;
                retorno = JsonConvert.DeserializeObject<RootObject>(teste2);
            }

            return retorno;
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
}
