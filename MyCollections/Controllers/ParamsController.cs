using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCollections.Models;
using System.Threading.Tasks;

namespace MyCollections.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Authorize]
    public class ParamsController : Controller
    {
        private readonly MyCollectionsContext _context;

        public ParamsController(MyCollectionsContext context)
        {
            _context = context;
        }

        // GET: Params
        public async Task<IActionResult> Index()
        {
            return View(await _context.Param.ToListAsync());
        }

        // GET: Params/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @param = await _context.Param.FindAsync(id);
            if (@param == null)
            {
                return NotFound();
            }
            return View(@param);
        }

        // POST: Params/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("key,value")] Param @param)
        {
            if (id != @param.key)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@param);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_context.Param.Find(@param.key) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@param);
        }

    }
}
