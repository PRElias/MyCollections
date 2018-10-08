using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyCollections.Models;

namespace MyCollections.Controllers
{
    public class UserController : Controller
    {
        private readonly MyCollectionsContext _context;

        public UserController(MyCollectionsContext context)
        {
            _context = context;
        }

        // GET: User/Edit/5
        public ActionResult Edit()
        {
            var userId = HttpContext.Session.GetString("loggedUserId");
            if (userId != null)
            {
                var user = _context.User.Find(userId);
                return View(user);
            }
            return NotFound();
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, User user)
        {
            try
            {
                var userToUpdate = _context.User.Find(user.Id);
                userToUpdate.steamUser = user.steamUser;
                _context.Update(userToUpdate);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}