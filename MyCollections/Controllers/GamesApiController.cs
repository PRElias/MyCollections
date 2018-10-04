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
    }
}