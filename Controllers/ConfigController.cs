using Microsoft.AspNetCore.Mvc;
using MyCollections.Models;
using MyCollections.Repositories;

namespace MyCollections.Controllers
{
    public class ConfigController : Controller
    {
        private readonly MyCollectionsRepository _db;
        public ConfigController([FromServices] MyCollectionsRepository db)
        {
            _db = db;
        }
        public IActionResult Edit()
        {
            var config = _db.FindConfig();
            return View();
        }
    }
}
