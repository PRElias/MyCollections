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
            var config = _db.GetAll();
            return View(config);
        }

        public IActionResult Upsert(Config config)
        {
            _db.Upsert(config);
            return RedirectToAction("Edit", "Config");
        }

    }
}
