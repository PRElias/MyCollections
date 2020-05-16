using Microsoft.AspNetCore.Mvc;
using System.Collections;
using MyCollections.Models;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System;
using System.Collections.Generic;
using MyCollections.Repositories;

namespace MyCollections.Controllers
{
    public class GamesController : Controller
    {
        private MyCollectionsRepository _db;
        private System.Collections.Generic.List<Game> games = new System.Collections.Generic.List<Game>();

        public GamesController([FromServices] MyCollectionsRepository db)
        {
            _db = db;
            LoadJson();
        }
        public IActionResult Index()
        {
            DownloadCovers();
            return View(games);
        }
        public void LoadJson()
        {
            using (StreamReader r = new StreamReader("docs/games/games.json"))
            {
                string json = r.ReadToEnd();
                games = JsonConvert.DeserializeObject<System.Collections.Generic.List<Game>>(json);
            }
        }

        public void DownloadCovers()
        {
            var gamesList = new List<Game>();

            foreach (var game in games)
            {
                if (game.LogoURL.Contains("http"))
                {
                    Uri uri = new Uri(game.LogoURL);
                    string  fileName = uri.Segments.GetValue(uri.Segments.Length - 1).ToString();
                    string newFileName = RemoveSpecialCharacters(game.Name) + ".jpg"; //game.Name.Substring(game.Name.LastIndexOf('.'));
                    WebClient myWebClient = new WebClient();
                    myWebClient.DownloadFile(uri, @"docs\games\covers\" + newFileName);
                    game.LogoURL = @"games/covers/" + newFileName;
                }

                gamesList.Add(game);
            }
            _db.SaveJson(gamesList);
        }
        
        public static string RemoveSpecialCharacters(string input)
        {
            var invalids = System.IO.Path.GetInvalidFileNameChars();
            return String.Join("", input.Split(invalids, StringSplitOptions.RemoveEmptyEntries) ).TrimEnd('.').Replace(" ", "");
        }
    }
}
