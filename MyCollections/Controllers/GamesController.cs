using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyCollections.Models;
using MyCollections.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCollections.Controllers
{
    
    public class GamesController : Controller
    {
        private readonly MyCollectionsContext _context;
        private readonly IMapper _mapper;

        public GamesController(MyCollectionsContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [Authorize]
        // GET: Games
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("loggedUserId");
            if (userId != null)
            {
                var user = _context.User.Find(userId);
                            ViewBag.userId = userId;
            ViewBag.userEmail = user.Email;

            }
            var myCollectionsContext = _context.Game.Include(g => g.Store).Include(g => g.System).Where(u => u.User.Id == userId);
            return View(await myCollectionsContext.ToListAsync());
        }

        [Authorize]
        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Game
                .Include(g => g.Store)
                .Include(g => g.System)
                .FirstOrDefaultAsync(m => m.GameID == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        [Authorize]
        // GET: Games/Create
        public IActionResult Create()
        {
            ViewData["StoreID"] = new SelectList(_context.Store, "StoreID", "Name");
            ViewData["SystemID"] = new SelectList(_context.System, "SystemID", "Name");
            return View();
        }

        [Authorize]
        // POST: Games/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Game game)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetString("loggedUserId");
                var user = _context.User.Find(userId);
                game.User = user;
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StoreID"] = new SelectList(_context.Store, "StoreID", "Name", game.StoreID);
            ViewData["SystemID"] = new SelectList(_context.System, "SystemID", "Name", game.SystemID);
            return View(game);
        }

        [Authorize]
        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Game.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            ViewData["StoreID"] = new SelectList(_context.Store, "StoreID", "Name", game.StoreID);
            ViewData["SystemID"] = new SelectList(_context.System, "SystemID", "Name", game.SystemID);
            return View(game);
        }

        [Authorize]
        // POST: Games/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GameID,Name,FriendlyName,Cover,Logo,SystemID,StoreID,BuyDate,Price,PlayedTime,Purchased,SteamApID,Active")] Game game)
        {
            if (id != game.GameID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.GameID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["StoreID"] = new SelectList(_context.Store, "StoreID", "Name", game.StoreID);
            ViewData["SystemID"] = new SelectList(_context.System, "SystemID", "Name", game.SystemID);
            return View(game);
        }

        [Authorize]
        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Game
                .Include(g => g.Store)
                .Include(g => g.System)
                .FirstOrDefaultAsync(m => m.GameID == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        [Authorize]
        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Game.FindAsync(id);
            _context.Game.Remove(game);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
            var userId = HttpContext.Session.GetString("loggedUserId");
            return _context.Game.Any(e => e.GameID == id && e.User.Id == userId);
        }

        // GET: GetFromSteam/5
        [HttpGet]
        [Route("GetFromSteam/{userId}")]
        public async Task<dynamic> GetFromSteam(string userId)
        {
            string steamkey = _context.Param.FirstOrDefault(p => p.key == "steam-key").value;
            string igdbkey = _context.Param.FirstOrDefault(p => p.key == "igdb-key").value;
            var user = _context.User.Find(userId);
            string steamid = user.steamUser;

            //Variáveis para exibir resumo pro cliente
            int gameNewCount = 0;
            int gameUpdateCount = 0;

            if (steamkey == string.Empty || steamid == string.Empty || steamid == null)
            {
                return StatusCode(204, "Chaves das API não informadas");
            }
            else
            {
                //Recupera todos os jogos do Steam do Usuário
                var games = await Steam.GetFromSteam(steamkey, steamid);

                if (games != null)
                {
                    foreach (var item in games.response.games)
                    {
                        int igdbId = 0;

                        if (igdbkey != null)
                        {
                            //igdbId = await UpdateGameDetails(igdbkey, item.name, item.appid);
                        }

                        //Atualiza tempos de jogo se ele já existir
                        var existingGame = _context.Game.FirstOrDefault(i => i.SteamApID == item.appid && i.User.Id == userId);
                        if (existingGame != null)
                        {
                            if (existingGame.Active == true)
                            {
                                gameUpdateCount++;
                                existingGame.PlayedTime = item.playtime_forever;
                                existingGame.IGDBId = igdbId;
                                _context.Game.Update(existingGame);
                                _context.SaveChanges();
                                continue;
                            }
                            continue;
                        }
                        
                        Game game = new Game();
                        gameNewCount++;
                        game.Name = item.name;
                        game.SteamApID = item.appid;
                        game.PlayedTime = item.playtime_forever;
                        if (item.img_logo_url != "" && item.img_logo_url != null)
                        {
                            game.Logo = "http://media.steampowered.com/steamcommunity/public/images/apps/" + item.appid + "/" + item.img_logo_url + ".jpg";
                        }
                        if (item.img_icon_url != "" && item.img_icon_url != null)
                        {
                            game.Cover = "http://media.steampowered.com/steamcommunity/public/images/apps/" + item.appid + "/" + item.img_icon_url + ".jpg";
                        }
                        game.StoreID = _context.Store.FirstOrDefault(s => s.Name == "Steam").StoreID;
                        game.SystemID = _context.System.FirstOrDefault(s => s.Name == "PC").SystemID;
                        game.Active = true;
                        game.User = user;
                        game.IGDBId = igdbId;
                        _context.Game.Add(game);
                        _context.SaveChanges();
                    }

                    return Ok("Importado com sucesso. Jogos novos: " + gameNewCount.ToString() + ", jogos atualizados: " + gameUpdateCount.ToString());
                }
                else
                {
                    return StatusCode(204, "Erro ao conectar no Steam");
                }
            }
        }

        private async Task<int> UpdateGameDetails(string igdbKey, string Name, int SteamApId)
        {
            var existingGameDetail = _context.GameDetails.FirstOrDefault(i => i.SteamApID == SteamApId && i.Name == Name);

            int igdbId = 0;

            if (existingGameDetail != null)
            {
                igdbId = (int)existingGameDetail.IGDBId;
            }
            else
            {
                var gameIGDBId = await IGDB.SearchIGDBByNameAndSteamId(igdbKey, Name, SteamApId);

                if (gameIGDBId.Length > 0)
                {
                    var gameDetails = await IGDB.GetFromIGDBByCode(igdbKey, gameIGDBId[0].Id.ToString());
                    GameDetails gd = new GameDetails();
                    gd.IDDBData = Newtonsoft.Json.JsonConvert.SerializeObject(gameDetails);
                    gd.IGDBId = Convert.ToInt32(gameIGDBId[0].Id);
                    gd.SteamApID = SteamApId;
                    gd.Name = Name;
                    gd.DateUpdated = DateTime.Now;
                    _context.GameDetails.Add(gd);
                    _context.SaveChanges();

                    igdbId = Convert.ToInt32(gameIGDBId[0].Id);
                }
            }
            return igdbId;
        }

        // GET: Games
        [Route("Games/ViewGames/{*id}")]
        public ViewResult ViewGames(string id)
        {
            var user = _context.User.Where(u => u.Email == id);
            if (user != null)
            {
                var games = _context.Game.Where(u => u.User.Email == id && u.Active == true).ToList().OrderBy(g => g.FriendlyName);
                return View(_mapper.Map<IEnumerable<GameApiDTO>>(games).Distinct());
            }
            else
            {
                //TODO: arrumar
                return View("ViewGames_empty");
            }
        }

        // GET: Games
        [Route("Games/ViewGames/")]
        public ViewResult ViewGames()
        {
            return View("ViewGames_empty");
        }
    }
}
