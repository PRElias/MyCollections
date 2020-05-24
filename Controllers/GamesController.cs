using Microsoft.AspNetCore.Mvc;
using MyCollections.Models;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System;
using System.Collections.Generic;
using MyCollections.Repositories;
using MyCollections.Services;
using System.Linq;

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
            UpdateGamesProperties();
            DownloadCovers();
            _db.SaveJson(games, @"docs/games/games.json");
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
                if (!String.IsNullOrEmpty(game.LogoURL))
                {
                    if (game.LogoURL.Contains("http"))
                    {
                        Uri uri = new Uri(game.LogoURL);
                        string fileName = uri.Segments.GetValue(uri.Segments.Length - 1).ToString();
                        if (fileName.Length == 4)
                        {
                            game.LogoURL = "";
                            continue;
                        }
                        string newFileName = Util.Helper.RemoveSpecialCharacters(game.Name) + ".jpg"; //game.Name.Substring(game.Name.LastIndexOf('.'));
                        WebClient myWebClient = new WebClient();
                        myWebClient.DownloadFile(uri, @"docs\games\covers\" + newFileName);
                        game.LogoURL = @"games/covers/" + newFileName;
                    }
                }

                //gamesList.Add(game);
            }
            // _db.SaveJson(gamesList, @"docs/games/games.json");
        }

        public List<Game> NewGamesFromSteam()
        {
            var steam = new Steam(_db.GetAll().steamKey, _db.GetAll().steamId);
            var allSteamGames = Steam.GetFromSteam().Result.response.games;
            var newGames = new System.Collections.Generic.List<Game>();
            foreach (var newGame in allSteamGames)
            {
                if (games.Exists(g => g.SteamApID == newGame.appid) == false)
                {
                    newGames.Add(new Game
                    {
                        Name = newGame.name,
                        LogoURL = "http://media.steampowered.com/steamcommunity/public/images/apps/" + newGame.appid + "/" + newGame.img_logo_url + ".jpg",
                        SteamOriginalImageURL = newGame.img_logo_url + ".jpg",
                        Store = "Steam",
                        System = "PC",
                        Disabled = false,
                        BuyDate = null,
                        Price = null,
                        PlayedTime = newGame.playtime_forever,
                        Purchased = true,
                        SteamApID = newGame.appid
                    });
                }
            }
            return newGames;
        }

        public void UpdateGamesProperties()
        {
            var steam = new Steam(_db.GetAll().steamKey, _db.GetAll().steamId);
            try
            {
                var allSteamGames = Steam.GetFromSteam().Result.response.games;

                foreach (var steamGame in allSteamGames)
                {
                    var gameFound = games.Find(g => g.Name == steamGame.name && g.Store == "Steam");
                    if (gameFound != null)
                    {
                        games[games.IndexOf(gameFound)].PlayedTime = steamGame.playtime_forever;
                        //games[games.IndexOf(gameFound)].SteamOriginalImageURL = steamGame.img_logo_url + ".jpg";
                    }
                }
                int id = 1;
                foreach (var savedGame in games)
                {
                    savedGame.GameID = id++;
                }

                // _db.SaveJson(games, @"docs/games/games.json");
                // return Ok();
            }
            catch (Exception error)
            {
                // return StatusCode(500, error);
            }
        }

        public IActionResult AutoNewGames()
        {
            var model = NewGamesFromSteam();
            return View(model);
        }

        [HttpPost]
        public IActionResult AutoNewGames(IEnumerable<Game> gameSelection)
        {
            var newGames = NewGamesFromSteam();
            for (int i = 0; i < gameSelection.Count(); i++)
            {
                if (gameSelection.ToList()[i].Selected == true)
                {
                    newGames.ToList()[i].Selected = false;
                    games.Add(newGames.ToList()[i]);
                }
            }
            _db.SaveJson(games, @"docs/games/games.json");
            return RedirectToAction("Index", "Games");
        }

        public IActionResult Edit(int id)
        {
            var game = games.Find(g => g.GameID == id);
            return View(game);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upsert(Game game)
        {
            var foundGame = games.FirstOrDefault(g => g.GameID == game.GameID);
            if (foundGame != null) 
            {
                if (!String.IsNullOrEmpty(game.SteamOriginalImageURL))
                {
                    if (game.SteamOriginalImageURL.Contains("http")) foundGame.LogoURL = game.SteamOriginalImageURL;
                }
                foundGame.Disabled = game.Disabled;
            }
            else
            {
                var cover = HttpContext.Request.Form.Files.GetFile("LogoURL");
                MyCollections.Util.File.UploadFile(cover, cover.FileName);
                game.LogoURL = "games/covers/" + cover.FileName;
                games.Add(game);
            }
            _db.SaveJson(games, @"docs/games/games.json");
            return RedirectToAction("Index", "Games");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            games.Remove(games.Find(j => j.GameID == id));
            _db.SaveJson(games, @"docs/games/games.json");
            return RedirectToAction("Index", "Games");
        }

        public IActionResult Commit()
        {
            Util.Helper.Commit();
            return RedirectToAction("Index", "Games");
        }
    }
}
