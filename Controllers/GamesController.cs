﻿using Microsoft.AspNetCore.Mvc;
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
                    if (fileName.Length == 4)
                    {
                        break;
                    }
                    string newFileName = Util.Helper.RemoveSpecialCharacters(game.Name) + ".jpg"; //game.Name.Substring(game.Name.LastIndexOf('.'));
                    WebClient myWebClient = new WebClient();
                    myWebClient.DownloadFile(uri, @"docs\games\covers\" + newFileName);
                    game.LogoURL = @"games/covers/" + newFileName;
                }

                gamesList.Add(game);
            }
            _db.SaveJson(gamesList, @"docs/games/games.json");
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
                    newGames.Add(new Game{
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

        public IActionResult UpdateGamesProperties()
        {
            var steam = new Steam(_db.GetAll().steamKey, _db.GetAll().steamId);
            try {
                var allSteamGames = Steam.GetFromSteam().Result.response.games;

                foreach (var steamGame in allSteamGames)
                {
                    var gameFound = games.Find(g => g.Name == steamGame.name && g.Store == "Steam");
                    if(gameFound != null)
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

                _db.SaveJson(games, @"docs/games/games.json");
                return Ok();
            }
            catch(Exception error) {
                return StatusCode(500, error);
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
            for (int i=0; i < gameSelection.Count(); i++)
            {
                if(gameSelection.ToList()[i].Selected == true)
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
        [HttpPost]
        public IActionResult Edit(Game game)
        {
            var foundGame = games.Find(g => g.GameID == game.GameID);
            foundGame = game;
            foundGame.LogoURL = game.SteamOriginalImageURL;
            _db.SaveJson(games, @"docs/games/games.json");
            return RedirectToAction("Index", "Games");;
        }

        public IActionResult Commit()
        {
            Util.Helper.Commit();
            return RedirectToAction("Index", "Games");
        }
    }
}