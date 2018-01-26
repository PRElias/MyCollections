﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCollections.Models;
using MyCollections.Services;

namespace MyCollections.Controllers
{
    [Produces("application/json")]
    [Route("api/Games")]
    public class GamesController : Controller
    {
        private readonly MyCollectionsContext _context;

        public GamesController(MyCollectionsContext context)
        {
            _context = context;
        }

        // GET: api/Games
        [HttpGet]
        public IEnumerable<Game> GetGames()
        {
            return _context.Game.Include("Store").Include("System");
        }

        // GET: api/Games/5
        [HttpGet("{name}")]
        public async Task<IActionResult> GetGame([FromRoute] string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var game = await _context.Game.SingleOrDefaultAsync(m => m.Name == name);

            if (game == null)
            {
                return NotFound();
            }

            return Ok(game);
        }

        [HttpPut]
        public async Task<dynamic> GetFromSteam()
        {
            string key = _context.Param.FirstOrDefault(p => p.key == "steam-key").value;
            string steamid = _context.Param.FirstOrDefault(p => p.key == "steam-steamid").value;

            if (key == string.Empty || steamid == string.Empty)
            {
                return StatusCode(204);
            }

            var games = await Steam.GetFromSteam(key, steamid);

            if (games != null)
            {
                foreach (var item in games.response.games)
                {
                    if (_context.Game.Any(g => g.SteamApID == item.appid))
                    {
                        var existingGame = _context.Game.FirstOrDefault(i => i.SteamApID == item.appid);
                        existingGame.PlayedTime = item.playtime_forever;
                        _context.Game.Update(existingGame);
                        _context.SaveChanges();
                        continue;
                    }

                    Game game = new Game();
                    game.Name = item.name;
                    game.SteamApID = item.appid;
                    game.PlayedTime = item.playtime_forever;
                    if (item.img_logo_url != null)
                    {
                        game.Logo = "http://media.steampowered.com/steamcommunity/public/images/apps/" + item.appid + "/" + item.img_logo_url + ".jpg";
                    }
                    game.StoreID = _context.Store.FirstOrDefault(s => s.Name == "Steam").StoreID;
                    game.SystemID = _context.System.FirstOrDefault(s => s.Name == "PC").SystemID;
                    game.Active = true;
                    _context.Game.Add(game);
                    _context.SaveChanges();
                }
            }

            return Ok("Importado com sucesso");
        }
    }
}