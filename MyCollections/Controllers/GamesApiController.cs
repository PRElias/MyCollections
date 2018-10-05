using AutoMapper;
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

        //// GET: api/Games/5
        //[HttpGet("{name}")]
        //public IEnumerable<dynamic> GetGame([FromRoute] string name)
        //{
        //    return _context.GamesDetailsView.Where(n => n.Game == name).ToList();
        //}
    }
}