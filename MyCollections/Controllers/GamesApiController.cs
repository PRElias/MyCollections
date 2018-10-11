using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCollections.Models;
using MyCollections.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyCollections.Controllers
{
    [Produces("application/json")]
    [Route("api/Games")]
    public class GamesApiController : Controller
    {
        private readonly MyCollectionsContext _context;
        private readonly IMapper _mapper;

        public GamesApiController(MyCollectionsContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Games
        [HttpGet("{email}")]
        public IEnumerable<GameApiDTO> GetUserGames([FromRoute] string email)
        {
            var games = _context.Game.Where(u => u.User.Email == email && u.Active == true).ToList();
            return _mapper.Map<IEnumerable<GameApiDTO>>(games);
        }

        // GET: api/Games/5
        [HttpGet("GetGame/{email}/{name}")]
        public IEnumerable<Game> GetGame([FromRoute] string email, string name)
        {
            //return _context.GamesDetailsView.Where(n => n.Game == name).ToList();
            var games = _context.Game.Include("User").Include("System").Include("Store").Where(u => u.User.Email == email && u.Active == true && u.Name == name).ToList().OrderBy(g => g.FriendlyName);
            var igdbId = games.FirstOrDefault<Game>().IGDBId;
            if (igdbId != null || igdbId > 0)
            {
                var gameDetails = _context.GameDetails.Where(g => g.IGDBId == igdbId);
            }
            return games;
        }
    }
}