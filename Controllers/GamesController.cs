using Microsoft.AspNetCore.Mvc;
using System.Collections;
using MyCollections.Models;
using System.IO;
using Newtonsoft.Json;

namespace MyCollections.Controllers
{
    public class GamesController : Controller
    {
        private System.Collections.Generic.List<Game> games = new System.Collections.Generic.List<Game>();
        public IActionResult Index()
        {
            LoadJson();
            return View(games);
        }
        public void LoadJson()
        {
            using (StreamReader r = new StreamReader("docs/games/games.json"))
            {
                string json = r.ReadToEnd();
                games = JsonConvert.DeserializeObject<System.Collections.Generic.List<Game>>(json);
            }
        }
    }
}
