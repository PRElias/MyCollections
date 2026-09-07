using Microsoft.AspNetCore.Mvc;
using MyCollections.Models;
using System.IO;
using Newtonsoft.Json;
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
        private static readonly HttpClient _imageSearchClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };

        public GamesController([FromServices] MyCollectionsRepository db)
        {
            _db = db;
            LoadJson();
        }
        public async Task<IActionResult> Index(bool semLogo = false)
        {
            UpdateGamesProperties();
            await DownloadCovers();
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

        public async Task DownloadCovers()
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
                        await MyCollections.Util.File.DownloadImageAsync(game.LogoURL, newFileName);
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
                if (HasSteamCopy(newGame) == false)
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

        private bool HasSteamCopy(SteamGame steamGame)
        {
            return games.Any(game =>
                String.Equals(game.Store, "Steam", StringComparison.OrdinalIgnoreCase) &&
                ((game.SteamApID.HasValue && game.SteamApID.Value == steamGame.appid) ||
                 ((!game.SteamApID.HasValue || game.SteamApID.Value == 0) && String.Equals(game.Name, steamGame.name, StringComparison.OrdinalIgnoreCase))));
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
            catch (Exception)
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

            var results = await SearchSteamImagesAsync(termo);
            if (results.Count < 12)
            {
                results.AddRange(await SearchImagesAsync(termo + " game cover"));
            }

            return Json(results
                .Where(item => !String.IsNullOrWhiteSpace(item.ImageUrl))
                .GroupBy(item => item.ImageUrl)
                .Select(group => group.First())
                .Take(12)
                .ToList());
        }

        [HttpPost]
        public async Task<IActionResult> SalvarLogoInternet(int gameId, string imageUrl, string sourceUrl)
        {
            var foundGame = games.FirstOrDefault(g => g.GameID == gameId);
            if (foundGame == null || String.IsNullOrWhiteSpace(imageUrl))
            {
                return RedirectToAction("Index", "Games");
            }

            try
            {
                var fileName = await MyCollections.Util.File.DownloadImageFromUrlAsync(imageUrl, gameId.ToString(), sourceUrl);
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

        private static async Task<List<ImageSearchResult>> SearchSteamImagesAsync(string term)
        {
            try
            {
                var url = "https://store.steampowered.com/api/storesearch/?term=" + Uri.EscapeDataString(term) + "&cc=us&l=pt-BR&v=1";
                using var request = CreateImageSearchRequest(url);
                using var response = await _imageSearchClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var parsed = JObject.Parse(json);
                var results = new List<ImageSearchResult>();

                foreach (var item in parsed["items"]?.Take(5) ?? Enumerable.Empty<JToken>())
                {
                    var appId = item.Value<int?>("id");
                    var name = item.Value<string>("name");
                    if (!appId.HasValue)
                    {
                        continue;
                    }

                    var imageUrls = new[]
                    {
                        $"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{appId.Value}/header.jpg",
                        $"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{appId.Value}/capsule_616x353.jpg"
                    };

                    foreach (var imageUrl in imageUrls)
                    {
                        var result = new ImageSearchResult
                        {
                            Title = "Steam - " + name,
                            ImageUrl = imageUrl,
                            ThumbnailUrl = imageUrl,
                            SourceUrl = "https://store.steampowered.com/app/" + appId.Value
                        };

                        if (await IsImageDownloadableAsync(result))
                        {
                            results.Add(result);
                        }
                    }
                }

                return results;
            }
            catch (Exception)
            {
                return new List<ImageSearchResult>();
            }
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

                var candidates = parsed["results"]?
                    .Take(30)
                    .Select(item => new ImageSearchResult
                    {
                        Title = item.Value<string>("title"),
                        ImageUrl = item.Value<string>("image"),
                        ThumbnailUrl = item.Value<string>("thumbnail"),
                        SourceUrl = item.Value<string>("url")
                    })
                    .Where(item => !String.IsNullOrWhiteSpace(item.ImageUrl) && !String.IsNullOrWhiteSpace(item.ThumbnailUrl))
                    .ToList() ?? new List<ImageSearchResult>();

                var validResults = new List<ImageSearchResult>();
                foreach (var candidate in candidates)
                {
                    if (await IsImageDownloadableAsync(candidate))
                    {
                        validResults.Add(candidate);
                    }

                    if (validResults.Count == 12)
                    {
                        break;
                    }
                }

                return validResults;
            }
            catch (Exception)
            {
                return new List<ImageSearchResult>();
            }
        }

        private static async Task<bool> IsImageDownloadableAsync(ImageSearchResult image)
        {
            try
            {
                using var request = CreateDownloadableImageRequest(image.ImageUrl, image.SourceUrl);
                using var response = await _imageSearchClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                var mediaType = response.Content.Headers.ContentType?.MediaType;
                return !String.IsNullOrWhiteSpace(mediaType) && mediaType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static HttpRequestMessage CreateDownloadableImageRequest(string imageUrl, string sourceUrl)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, imageUrl);
            request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0 Safari/537.36");
            request.Headers.Accept.ParseAdd("image/avif,image/webp,image/apng,image/svg+xml,image/*,*/*;q=0.8");

            if (!String.IsNullOrWhiteSpace(sourceUrl) && Uri.TryCreate(sourceUrl, UriKind.Absolute, out var sourceUri))
            {
                request.Headers.Referrer = sourceUri;
            }

            return request;
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
