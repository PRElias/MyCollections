using Microsoft.AspNetCore.Mvc;

namespace MyCollections.Controllers
{
    public class GamesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
