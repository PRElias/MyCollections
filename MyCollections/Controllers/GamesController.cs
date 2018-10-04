using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyCollections.Models;
using MyCollections.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyCollections.Controllers
{
    [Authorize]
    public class GamesController : Controller
    {
        private readonly MyCollectionsContext _context;

        public GamesController(MyCollectionsContext context)
        {
            _context = context;
        }

        // GET: Games
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("loggedUserId");
            //if (userId == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            ViewBag.userId = userId;
            var myCollectionsContext = _context.Game.Include(g => g.Store).Include(g => g.System).Where(u => u.User.Id == userId);
            return View(await myCollectionsContext.ToListAsync());
        }

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

        // GET: Games/Create
        public IActionResult Create()
        {
            ViewData["StoreID"] = new SelectList(_context.Store, "StoreID", "Name");
            ViewData["SystemID"] = new SelectList(_context.System, "SystemID", "Name");
            return View();
        }

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
                var user = _context.Users.Find(userId);
                game.User = user;
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StoreID"] = new SelectList(_context.Store, "StoreID", "Name", game.StoreID);
            ViewData["SystemID"] = new SelectList(_context.System, "SystemID", "Name", game.SystemID);
            return View(game);
        }

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

        [Route("GetFromSteam/{userId}")]
        public async Task<dynamic> GetFromSteam(string userId)
        {
            string steamkey = _context.Param.FirstOrDefault(p => p.key == "steam-key").value;
            //string steamid = _context.Param.FirstOrDefault(p => p.key == "steam-steamid").value;
            string igdbkey = _context.Param.FirstOrDefault(p => p.key == "igdb-key").value;
            var user = _context.Users.Find(userId);
            string steamid = user.steamUser;

            int gameNewCount = 0;
            int gameUpdateCount = 0;

            if (steamkey == string.Empty || steamid == string.Empty || steamid == null || igdbkey == string.Empty)
            {
                return StatusCode(204, "Chaves das API não informadas");
            }
            else
            {
                var games = await Steam.GetFromSteam(steamkey, steamid);

                if (games != null)
                {
                    foreach (var item in games.response.games)
                    {
                        //Verifica se o jogo tem IGDB pelo Nome e SteamID
                        var existingIgdbGame = _context.Game.FirstOrDefault(i => i.Name == item.name && i.IGDBId == null && i.User.Id == userId);

                        if (existingIgdbGame != null)
                        {
                            var gameIGDBId = await IGDB.SearchIGDBByNameAndSteamId(igdbkey, item.name, existingIgdbGame.SteamApID.ToString());
                            if (gameIGDBId.Length > 0)
                            {
                                var gameDetails = await IGDB.GetFromIGDBByCode(igdbkey, gameIGDBId[0].Id.ToString());
                                existingIgdbGame.GameDetails = Newtonsoft.Json.JsonConvert.SerializeObject(gameDetails);
                                existingIgdbGame.IGDBId = Convert.ToInt32(gameIGDBId[0].Id);
                                _context.Game.Update(existingIgdbGame);
                                _context.SaveChanges();
                            }
                        }

                        if (_context.Game.Any(g => g.SteamApID == item.appid))
                        {
                            var existingGame = _context.Game.FirstOrDefault(i => i.SteamApID == item.appid && i.User.Id == userId);
                            gameUpdateCount++;
                            existingGame.PlayedTime = item.playtime_forever;
                            _context.Game.Update(existingGame);
                            _context.SaveChanges();
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
    }
}
