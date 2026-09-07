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
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace MyCollections.Controllers
{
    public class GamesController : Controller
    {
        private MyCollectionsRepository _db;
        private System.Collections.Generic.List<Game> games = new System.Collections.Generic.List<Game>();
        private static readonly HttpClient _imageSearchClient = new HttpClient();

        public GamesController([FromServices] MyCollectionsRepository db)
        {
            _db = db;
            LoadJson();
        }
        public IActionResult Index(bool semLogo = false)
        {
            UpdateGamesProperties();
            DownloadCovers();
            _db.SaveJson(games, @"docs/games/games.json");
            ViewBag.SemLogo = semLogo;
            return View(semLogo ? games.Where(g => !g.Disabled && GameHasNoLogo(g)).ToList() : games);
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
            //Verificar se o jogo já existe, então é um update
            var foundGame = games.FirstOrDefault(g => g.GameID == game.GameID);
            if (foundGame != null) 
            {
                if (!String.IsNullOrEmpty(game.SteamOriginalImageURL))
                {
                    if (game.SteamOriginalImageURL.Contains("http"))
                    {
                        string fileName = game.GameID.ToString() + ".png";
                        games[games.IndexOf(foundGame)].LogoURL = "games/covers/" + fileName;
                        _ = MyCollections.Util.File.DownloadImageAsync(game.SteamOriginalImageURL, fileName);
                    }
                }

                var cover = HttpContext.Request.Form.Files.GetFile("LogoURL");
                if (cover != null)
                {
                    MyCollections.Util.File.UploadFile(cover, cover.FileName);
                    games[games.IndexOf(foundGame)].LogoURL = "games/covers/" + cover.FileName;
                }
                
                games[games.IndexOf(foundGame)].Disabled = game.Disabled;
                games[games.IndexOf(foundGame)].Name = game.Name;
            }
            else
            {
                games.Add(game);
            }

            _db.SaveJson(games, @"docs/games/games.json");
            return RedirectToAction("Index", "Games");
        }

        public async Task<IActionResult> AtualizarLogosSteam()
        {
            var updated = 0;
            var ignored = 0;

            foreach (var game in games.Where(g => !g.Disabled && GameHasNoLogo(g) && g.SteamApID.HasValue && g.SteamApID.Value > 0))
            {
                var fileName = game.GameID.ToString() + ".png";
                var steamImageUrl = GetSteamHeaderUrl(game);

                try
                {
                    await MyCollections.Util.File.DownloadImageAsync(steamImageUrl, fileName);
                    game.LogoURL = "games/covers/" + fileName;
                    updated++;
                }
                catch (HttpRequestException)
                {
                    ignored++;
                }
            }

            _db.SaveJson(games, @"docs/games/games.json");
            TempData["Mensagem"] = $"Logos atualizados: {updated}. Steam sem imagem/resposta: {ignored}.";
            return RedirectToAction("Index", "Games", new { semLogo = true });
        }

        public async Task<IActionResult> PesquisarImagens(string termo)
        {
            if (String.IsNullOrWhiteSpace(termo))
            {
                return Json(new List<ImageSearchResult>());
            }

            var results = await SearchImagesAsync(termo + " game cover");
            return Json(results);
        }

        [HttpPost]
        public async Task<IActionResult> SalvarLogoInternet(int gameId, string imageUrl)
        {
            var foundGame = games.FirstOrDefault(g => g.GameID == gameId);
            if (foundGame == null || String.IsNullOrWhiteSpace(imageUrl))
            {
                return RedirectToAction("Index", "Games");
            }

            try
            {
                var fileName = await MyCollections.Util.File.DownloadImageFromUrlAsync(imageUrl, gameId.ToString());
                games[games.IndexOf(foundGame)].LogoURL = "games/covers/" + fileName;
                _db.SaveJson(games, @"docs/games/games.json");
                TempData["Mensagem"] = "Logo atualizado com a imagem escolhida.";
            }
            catch (Exception)
            {
                TempData["Mensagem"] = "Não foi possível salvar a imagem escolhida.";
            }

            return RedirectToAction("Edit", "Games", new { id = gameId });
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

        public async Task FindGameOnSteamAsync()
        {
            foreach (var game in games)
            {
                if (game.SteamApID == null)
                {
                    game.SteamApID = Convert.ToInt32(await Steam.SearchGameByName(game.Name));
                }
            }
            _db.SaveJson(games, @"docs/games/games.json");

        }

        private static async Task<List<ImageSearchResult>> SearchImagesAsync(string query)
        {
            try
            {
                using var pageRequest = CreateImageSearchRequest("https://duckduckgo.com/?q=" + Uri.EscapeDataString(query) + "&iax=images&ia=images");
                var page = await _imageSearchClient.SendAsync(pageRequest);
                page.EnsureSuccessStatusCode();
                var html = await page.Content.ReadAsStringAsync();
                var vqdMatch = Regex.Match(html, "vqd=['\\\"]?([^'\\\"&]+)");
                if (!vqdMatch.Success)
                {
                    return new List<ImageSearchResult>();
                }

                var url = "https://duckduckgo.com/i.js?l=us-en&o=json&q=" + Uri.EscapeDataString(query) + "&vqd=" + Uri.EscapeDataString(vqdMatch.Groups[1].Value) + "&f=,,,,,&p=1";
                using var imageRequest = CreateImageSearchRequest(url);
                var response = await _imageSearchClient.SendAsync(imageRequest);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var parsed = JObject.Parse(json);

                return parsed["results"]?
                    .Take(12)
                    .Select(item => new ImageSearchResult
                    {
                        Title = item.Value<string>("title"),
                        ImageUrl = item.Value<string>("image"),
                        ThumbnailUrl = item.Value<string>("thumbnail"),
                        SourceUrl = item.Value<string>("url")
                    })
                    .Where(item => !String.IsNullOrWhiteSpace(item.ImageUrl) && !String.IsNullOrWhiteSpace(item.ThumbnailUrl))
                    .ToList() ?? new List<ImageSearchResult>();
            }
            catch (Exception)
            {
                return new List<ImageSearchResult>();
            }
        }

        private static HttpRequestMessage CreateImageSearchRequest(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.UserAgent.ParseAdd("Mozilla/5.0 MyCollections/1.0");
            request.Headers.Referrer = new Uri("https://duckduckgo.com/");
            return request;
        }

        private static string GetSteamHeaderUrl(Game game)
        {
            return $"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{game.SteamApID}/header.jpg";
        }

        private static bool GameHasNoLogo(Game game)
        {
            if (String.IsNullOrWhiteSpace(game.LogoURL))
            {
                return true;
            }

            if (game.LogoURL.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var logoPath = game.LogoURL.Replace('\\', '/');
            if (logoPath.StartsWith("./", StringComparison.Ordinal))
            {
                logoPath = logoPath.Substring(2);
            }
            if (logoPath.StartsWith("docs/", StringComparison.OrdinalIgnoreCase))
            {
                logoPath = logoPath.Substring(5);
            }

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "docs", logoPath.Replace('/', Path.DirectorySeparatorChar));
            return !System.IO.File.Exists(fullPath);
        }
    }
}
