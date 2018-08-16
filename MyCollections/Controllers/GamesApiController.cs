using System;
using System.Collections.Generic;
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
    public class GamesApiController : Controller
    {
        private readonly MyCollectionsContext _context;

        public GamesApiController(MyCollectionsContext context)
        {
            _context = context;
        }

        // GET: api/Games
        [HttpGet]
        public IEnumerable<GamesDistinctView> GetGames()
        {
            return _context.GamesDistinctView.ToList();
        }

        // GET: api/Games/5
        [HttpGet("{name}")]
        public IEnumerable<dynamic> GetGame([FromRoute] string name)
        {
            return _context.GamesDetailsView.Where(n => n.Game == name).ToList();
        }

        [HttpGet("GetFromServices")]
        public async Task<dynamic> GetFromServices()
        {
            string steamkey = _context.Param.FirstOrDefault(p => p.key == "steam-key").value;
            string steamid = _context.Param.FirstOrDefault(p => p.key == "steam-steamid").value;
            string igdbkey = _context.Param.FirstOrDefault(p => p.key == "igdb-key").value;

            int gameNewCount = 0;
            int gameUpdateCount = 0;

            if (steamkey == string.Empty || steamid == string.Empty || igdbkey == string.Empty)
            {
                return StatusCode(204, "Chaves das API não informadas");
            }

            var games = await Steam.GetFromSteam(steamkey, steamid);

            if (games != null)
            {
                foreach (var item in games.response.games)
                {
                    //Verifica se o jogo tem IGDB pelo Nome e SteamID
                    var existingIgdbGame = _context.Game.FirstOrDefault(i => i.Name == item.name && i.IGDBId == null);

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
                        var existingGame = _context.Game.FirstOrDefault(i => i.SteamApID == item.appid);
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
                    _context.Game.Add(game);
                    _context.SaveChanges();
                }
            }

            return Ok("Importado com sucesso. Jogos novos: " + gameNewCount.ToString() + ", jogos atualizados: " + gameUpdateCount.ToString());
        }

        // [HttpGet("GetFromIGDB/{game}")]
        // public async Task<dynamic> GetFromIGDB([FromRoute] string game)
        // {
        //     string igdbkey = _context.Param.FirstOrDefault(p => p.key == "igdb-key").value;
        //     var existingGame = _context.Game.FirstOrDefault(i => i.Name == game && i.IGDBId == null);
        //     if (existingGame != null)
        //     {
        //         var gameIGDBId = await IGDB.SearchIGDBByNameAndSteamId(igdbkey, game, existingGame.SteamApID.ToString());
        //         var gameDetails = await IGDB.GetFromIGDBByCode(igdbkey, gameIGDBId[0].Id.ToString());
        //         existingGame.GameDetails = Newtonsoft.Json.JsonConvert.SerializeObject(gameDetails);
        //         existingGame.IGDBId = Convert.ToInt32(gameIGDBId[0].Id);
        //         _context.Game.Update(existingGame);
        //         _context.SaveChanges();
        //         return Ok(gameDetails);
        //     }
        //     else
        //     {
        //         return NotFound("Não encontrado ou já atualizado");
        //     }
        // }
    }
}