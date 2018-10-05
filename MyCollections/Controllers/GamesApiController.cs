using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCollections.Models;
using MyCollections.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        [HttpGet("{email}")]
        public IEnumerable<Game> GetGames([FromRoute] string email)
        {
            //return _context.GamesDistinctView.ToList();
            return _context.Game.Where(u => u.User.Email == email && u.Active == true).ToList();
        }
    }
}