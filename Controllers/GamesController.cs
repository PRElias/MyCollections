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
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace MyCollections.Controllers
{
    public class GamesController : Controller
    {
        private MyCollectionsRepository _db;
        private System.Collections.Generic.List<Game> games = new System.Collections.Generic.List<Game>();
        private static readonly HttpClient _imageSearchClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
        private static readonly HttpClient _igdbClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

        public GamesController([FromServices] MyCollectionsRepository db)
        {
            _db = db;
            LoadJson();
        }
        public async Task<IActionResult> Index(bool semLogo = false)
        {
            NormalizeGameIds();
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
        private void NormalizeGameIds()
        {
            int id = 1;
            foreach (var savedGame in games)
            {
                savedGame.GameID = id++;
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
            var gameDetails = LoadGameDetails().FirstOrDefault(d => d.FriendlyName == game?.FriendlyName);
            ViewBag.ExophaseUrl = gameDetails?.ExophaseUrl;
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
            if (results.Count < 12)
            {
                results.AddRange(await SearchPageImagesAsync(termo + " game cover"));
            }

            return Json(results
                .Where(item => !String.IsNullOrWhiteSpace(item.ImageUrl))
                .GroupBy(item => item.ImageUrl)
                .Select(group => group.First())
                .Take(12)
                .ToList());
        }

        [HttpPost]
        public async Task<IActionResult> SalvarLogoInternet(int gameId, string imageUrl, string thumbnailUrl, string sourceUrl)
        {
            var foundGame = games.FirstOrDefault(g => g.GameID == gameId);
            if (foundGame == null || String.IsNullOrWhiteSpace(imageUrl))
            {
                return RedirectToAction("Index", "Games");
            }

            try
            {
                var fileName = await DownloadSelectedImageAsync(imageUrl, thumbnailUrl, sourceUrl, gameId.ToString());
                games[games.IndexOf(foundGame)].LogoURL = "games/covers/" + fileName;
                _db.SaveJson(games, @"docs/games/games.json");
                TempData["Mensagem"] = "Logo atualizado com a imagem escolhida.";
            }
            catch (Exception)
            {
                TempData["Mensagem"] = "Não foi possível salvar a imagem escolhida nem a miniatura dela.";
            }

            return RedirectToAction("Edit", "Games", new { id = gameId });
        }
        public async Task<IActionResult> AtualizarDetalhesIgdb(int? id)
        {
            var config = _db.GetAll();
            if (String.IsNullOrWhiteSpace(config.igdbClientId) || String.IsNullOrWhiteSpace(config.igdbClientSecret))
            {
                TempData["Mensagem"] = "Informe IGDB Client ID e IGDB Client Secret em Configurações.";
                return id.HasValue ? RedirectToAction("Edit", "Games", new { id = id.Value }) : RedirectToAction("Index", "Games");
            }

            var details = LoadGameDetails();
            var targets = id.HasValue
                ? games.Where(g => g.GameID == id.Value).ToList()
                : games.Where(g => !g.Disabled)
                    .GroupBy(g => g.FriendlyName)
                    .Select(group => group.First())
                    .Where(g => details.Any(d => d.FriendlyName == g.FriendlyName) == false)
                    .ToList();

            var updated = 0;
            var notFound = 0;
            var errors = 0;

            foreach (var game in targets)
            {
                try
                {
                    var detail = await SearchIgdbGameDetailsAsync(game, config);
                    if (detail == null)
                    {
                        notFound++;
                    }
                    else
                    {
                        UpsertGameDetails(details, detail);
                        foreach (var copy in games.Where(g => g.FriendlyName == game.FriendlyName))
                        {
                            copy.IGDBId = detail.IGDBId;
                        }
                        updated++;
                    }
                }
                catch (Exception)
                {
                    errors++;
                }

                await Task.Delay(300);
            }

            SaveGameDetails(details);
            if (updated > 0)
            {
                _db.SaveJson(games, @"docs/games/games.json");
            }

            TempData["Mensagem"] = $"Detalhes IGDB atualizados: {updated}. Não encontrados: {notFound}. Erros: {errors}.";
            return id.HasValue ? RedirectToAction("Edit", "Games", new { id = id.Value }) : RedirectToAction("Index", "Games");
        }

        [HttpPost]
        public IActionResult SalvarExophaseUrl(int gameId, string exophaseUrl)
        {
            var foundGame = games.FirstOrDefault(g => g.GameID == gameId);
            if (foundGame == null)
            {
                TempData["Mensagem"] = "Jogo não encontrado.";
                return RedirectToAction("Index", "Games");
            }

            if (!String.IsNullOrWhiteSpace(exophaseUrl) &&
                (Uri.TryCreate(exophaseUrl, UriKind.Absolute, out var uri) == false || uri.Host.EndsWith("exophase.com", StringComparison.OrdinalIgnoreCase) == false))
            {
                TempData["Mensagem"] = "Informe uma URL válida do Exophase.";
                return RedirectToAction("Edit", "Games", new { id = gameId });
            }

            var details = LoadGameDetails();
            var detail = details.FirstOrDefault(d => d.FriendlyName == foundGame.FriendlyName);
            if (detail == null)
            {
                detail = new GameDetails
                {
                    GameDetailsID = details.Any() ? details.Max(d => d.GameDetailsID) + 1 : 1,
                    FriendlyName = foundGame.FriendlyName,
                    Name = foundGame.Name
                };
                details.Add(detail);
            }

            detail.ExophaseUrl = String.IsNullOrWhiteSpace(exophaseUrl) ? null : exophaseUrl.Trim();
            detail.DateUpdated = DateTime.Now;
            SaveGameDetails(details);

            TempData["Mensagem"] = String.IsNullOrWhiteSpace(detail.ExophaseUrl) ? "Link do Exophase removido." : "Link do Exophase salvo.";
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

                        if (await HasDownloadableImageAsync(result))
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
        private static async Task<List<ImageSearchResult>> SearchPageImagesAsync(string query)
        {
            try
            {
                var url = "https://www.bing.com/search?q=" + Uri.EscapeDataString(query);
                using var request = CreateImageSearchRequest(url);
                using var response = await _imageSearchClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var html = await response.Content.ReadAsStringAsync();
                var pageUrls = Regex.Matches(html, "<h2[^>]*>\\s*<a[^>]+href=\\\"(?<url>https?://[^\\\"]+)", RegexOptions.IgnoreCase)
                    .Select(match => match.Groups["url"].Value)
                    .Where(pageUrl => !pageUrl.Contains("bing.com", StringComparison.OrdinalIgnoreCase))
                    .Distinct()
                    .Take(8)
                    .ToList();

                var results = new List<ImageSearchResult>();
                foreach (var pageUrl in pageUrls)
                {
                    var image = await GetOpenGraphImageAsync(pageUrl);
                    if (image != null)
                    {
                        results.Add(image);
                    }

                    if (results.Count == 12)
                    {
                        break;
                    }
                }

                return results;
            }
            catch (Exception)
            {
                return new List<ImageSearchResult>();
            }
        }

        private static async Task<ImageSearchResult> GetOpenGraphImageAsync(string pageUrl)
        {
            try
            {
                using var request = CreateImageSearchRequest(pageUrl);
                using var response = await _imageSearchClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var html = await response.Content.ReadAsStringAsync();
                var title = GetMetaContent(html, "og:title") ?? Regex.Replace(Regex.Match(html, "<title[^>]*>(?<title>.*?)</title>", RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups["title"].Value, "\\s+", " ").Trim();
                var imageUrl = GetMetaContent(html, "og:image") ?? GetMetaContent(html, "twitter:image");
                if (String.IsNullOrWhiteSpace(imageUrl))
                {
                    return null;
                }

                imageUrl = new Uri(new Uri(pageUrl), imageUrl).ToString();
                return new ImageSearchResult
                {
                    Title = title,
                    ImageUrl = imageUrl,
                    ThumbnailUrl = imageUrl,
                    SourceUrl = pageUrl
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string GetMetaContent(string html, string property)
        {
            var pattern = "<meta[^>]+(?:property|name)=['\\\"]" + Regex.Escape(property) + "['\\\"][^>]+content=['\\\"](?<content>[^'\\\"]+)['\\\"]";
            var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
            return match.Success ? System.Net.WebUtility.HtmlDecode(match.Groups["content"].Value) : null;
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

                return candidates.Take(12).ToList();
            }
            catch (Exception)
            {
                return new List<ImageSearchResult>();
            }
        }

        private static async Task<string> DownloadSelectedImageAsync(string imageUrl, string thumbnailUrl, string sourceUrl, string baseFileName)
        {
            try
            {
                return await MyCollections.Util.File.DownloadImageFromUrlAsync(imageUrl, baseFileName, sourceUrl);
            }
            catch (Exception) when (!String.IsNullOrWhiteSpace(thumbnailUrl) && !String.Equals(imageUrl, thumbnailUrl, StringComparison.OrdinalIgnoreCase))
            {
                return await MyCollections.Util.File.DownloadImageFromUrlAsync(thumbnailUrl, baseFileName, sourceUrl);
            }
        }

        private static async Task<bool> HasDownloadableImageAsync(ImageSearchResult image)
        {
            return await IsImageUrlDownloadableAsync(image.ImageUrl, image.SourceUrl) ||
                   await IsImageUrlDownloadableAsync(image.ThumbnailUrl, image.SourceUrl);
        }

        private static async Task<bool> IsImageUrlDownloadableAsync(string imageUrl, string sourceUrl)
        {
            if (String.IsNullOrWhiteSpace(imageUrl))
            {
                return false;
            }

            try
            {
                using var request = CreateDownloadableImageRequest(imageUrl, sourceUrl);
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

        private static List<GameDetails> LoadGameDetails()
        {
            var path = Path.Combine("docs", "games", "games-details.json");
            if (!System.IO.File.Exists(path))
            {
                return new List<GameDetails>();
            }

            var json = System.IO.File.ReadAllText(path);
            if (String.IsNullOrWhiteSpace(json))
            {
                return new List<GameDetails>();
            }

            return JsonConvert.DeserializeObject<List<GameDetails>>(json) ?? new List<GameDetails>();
        }

        private static void SaveGameDetails(List<GameDetails> details)
        {
            var path = Path.Combine("docs", "games", "games-details.json");
            var json = JsonConvert.SerializeObject(details.OrderBy(d => d.Name).ToList(), Formatting.Indented);
            System.IO.File.WriteAllText(path, json);
        }

        private static void UpsertGameDetails(List<GameDetails> details, GameDetails detail)
        {
            var existing = details.FirstOrDefault(d => d.FriendlyName == detail.FriendlyName);
            if (existing == null)
            {
                detail.GameDetailsID = details.Any() ? details.Max(d => d.GameDetailsID) + 1 : 1;
                details.Add(detail);
                return;
            }

            detail.GameDetailsID = existing.GameDetailsID;
            detail.ExophaseUrl = String.IsNullOrWhiteSpace(detail.ExophaseUrl) ? existing.ExophaseUrl : detail.ExophaseUrl;
            var index = details.IndexOf(existing);
            details[index] = detail;
        }

        private static async Task<GameDetails> SearchIgdbGameDetailsAsync(Game game, Config config)
        {
            var token = await GetIgdbAccessTokenAsync(config);
            var body = "search \"" + EscapeIgdbString(CleanGameNameForSearch(game.Name)) + "\"; " +
                       "fields id,name,summary,storyline,first_release_date,url,genres.name,involved_companies.company.name,involved_companies.developer,involved_companies.publisher,total_rating; " +
                       "limit 5;";

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.igdb.com/v4/games");
            request.Headers.Add("Client-ID", config.igdbClientId);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Accept.ParseAdd("application/json");
            request.Content = new StringContent(body, Encoding.UTF8, "text/plain");

            using var response = await _igdbClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var results = JArray.Parse(json);
            if (!results.Any())
            {
                return null;
            }

            var normalizedSearch = NormalizeGameName(CleanGameNameForSearch(game.Name));
            var selected = results.FirstOrDefault(item => NormalizeGameName(item.Value<string>("name")) == normalizedSearch) ?? results.First();
            var detail = new GameDetails
            {
                FriendlyName = game.FriendlyName,
                Name = selected.Value<string>("name") ?? game.Name,
                SteamApID = game.SteamApID,
                IGDBId = selected.Value<int?>("id"),
                IGDBUrl = selected.Value<string>("url"),
                Summary = selected.Value<string>("summary"),
                Storyline = selected.Value<string>("storyline"),
                FirstReleaseDate = GetDateFromUnixTime(selected.Value<long?>("first_release_date")),
                Genres = GetNamedChildren(selected, "genres"),
                TotalRating = selected.Value<double?>("total_rating"),
                IDDBData = selected.ToString(Formatting.None),
                DateUpdated = DateTime.Now
            };

            FillCompanies(selected, detail);
            return detail;
        }

        private static async Task<string> GetIgdbAccessTokenAsync(Config config)
        {
            var url = "https://id.twitch.tv/oauth2/token?client_id=" + Uri.EscapeDataString(config.igdbClientId) +
                      "&client_secret=" + Uri.EscapeDataString(config.igdbClientSecret) +
                      "&grant_type=client_credentials";
            using var response = await _igdbClient.PostAsync(url, null);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var parsed = JObject.Parse(json);
            return parsed.Value<string>("access_token");
        }

        private static void FillCompanies(JToken selected, GameDetails detail)
        {
            var companies = selected["involved_companies"] as JArray;
            if (companies == null)
            {
                return;
            }

            foreach (var involvedCompany in companies)
            {
                var name = involvedCompany["company"]?.Value<string>("name");
                if (String.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                if (involvedCompany.Value<bool?>("developer") == true && detail.Developers.Contains(name) == false)
                {
                    detail.Developers.Add(name);
                }
                if (involvedCompany.Value<bool?>("publisher") == true && detail.Publishers.Contains(name) == false)
                {
                    detail.Publishers.Add(name);
                }
            }
        }

        private static List<string> GetNamedChildren(JToken item, string property)
        {
            var values = item[property] as JArray;
            if (values == null)
            {
                return new List<string>();
            }

            return values
                .Select(value => value.Value<string>("name"))
                .Where(value => !String.IsNullOrWhiteSpace(value))
                .Distinct()
                .ToList();
        }

        private static DateTime? GetDateFromUnixTime(long? unixTime)
        {
            return unixTime.HasValue ? DateTimeOffset.FromUnixTimeSeconds(unixTime.Value).DateTime : null;
        }

        private static string CleanGameNameForSearch(string name)
        {
            var clean = Regex.Replace(name ?? String.Empty, "[®™©]", String.Empty);
            clean = Regex.Replace(clean, "\\s*\\((PC|Windows|Xbox Series X\\|S|Xbox One e Xbox Series X\\|S)\\)\\s*$", String.Empty, RegexOptions.IgnoreCase);
            clean = Regex.Replace(clean, "\\s*-\\s*(PC|Windows)\\s*$", String.Empty, RegexOptions.IgnoreCase);
            clean = Regex.Replace(clean, "\\s+para\\s+(Xbox|Windows|Xbox One e Xbox Series X\\|S|Xbox Series X\\|S)\\s*$", String.Empty, RegexOptions.IgnoreCase);
            clean = Regex.Replace(clean, "^Edi[cç][aã]o\\s+(Standard|Padr[aã]o)\\s+do\\s+", String.Empty, RegexOptions.IgnoreCase);
            clean = Regex.Replace(clean, "\\s+(Edi[cç][aã]o\\s+(Standard|Padr[aã]o)|Standard Edition|Edi[cç][aã]o Digital Deluxe|Digital Deluxe Edition|Deluxe Edition|Definitive Edition|Gold Edition|Game of the Year Edition)\\s*$", String.Empty, RegexOptions.IgnoreCase);
            return Regex.Replace(clean, "\\s+", " ").Trim();
        }

        private static string NormalizeGameName(string name)
        {
            var clean = CleanGameNameForSearch(name);
            clean = Regex.Replace(clean, "[^a-zA-Z0-9]+", " ").Trim().ToUpperInvariant();
            return clean;
        }

        private static string EscapeIgdbString(string value)
        {
            return (value ?? String.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");
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

