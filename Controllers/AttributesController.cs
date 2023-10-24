using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MongoBasedWeb.Models;
using MongoBasedWeb.Services;
using MongoDB.Driver;

namespace MongoBasedWeb.Controllers
{
    public class AttributesController : Controller
    {
        private readonly IMongoCollection<Attributes> _attrs;

        public AttributesController(MongoService service)
        {
            _attrs = service.GetCollection<Attributes>();
        }
        public async Task<IActionResult> Index()
        {
            return _attrs != null ? View(await(await _attrs.FindAsync(_ => true)).ToListAsync()) : Problem("Entity set is null.");
        }

        public async Task<IActionResult> Create()
        {
            if (_attrs == null)
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Cuteness")] Attributes attr)
        {
            if (ModelState.IsValid)
            {
                await _attrs.InsertOneAsync(attr);
                return RedirectToAction(nameof(Index));
            }
            return await Create();
        }

        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null || _attrs == null)
            {
                return NotFound();
            }

            var attr = (await _attrs.FindAsync(m => m.Id == id)).FirstOrDefault();
            if (attr == null)
            {
                return NotFound();
            }
            return View(attr);
        }

        // POST: Cats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Cuteness")] Attributes attr)
        {
            if (id != attr.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _attrs.ReplaceOneAsync(m => m.Id == id, attr);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CatsExistsAsync(attr.Id).Result)
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
            return View(attr);
        }
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null || _attrs == null)
            {
                return NotFound();
            }

            var attr = (await _attrs.FindAsync(m => m.Id == id)).FirstOrDefault();
            if (attr == null)
            {
                return NotFound();
            }

            return View(attr);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_attrs == null)
            {
                return Problem("Entity set is null.");
            }
            var cats = (await _attrs.FindAsync(m => m.Id == id)).FirstOrDefault();
            if (cats != null)
            {
                await _attrs.DeleteOneAsync(x => x.Id == id);
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CatsExistsAsync(string id)
        {
            return (await _attrs?.FindAsync(e => e.Id == id)!).Any();
        }
    }
}
