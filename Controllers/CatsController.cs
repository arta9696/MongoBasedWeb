using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoBasedWeb.Models;
using MongoBasedWeb.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoBasedWeb.Controllers
{
    public class CatsController : Controller
    {
        private readonly IMongoCollection<Cats> _cats;
        private readonly IMongoCollection<Attributes> _attrs;

        public CatsController(MongoService service)
        {
            _cats = service.GetCollection<Cats>();
            _attrs = service.GetCollection<Attributes>();
        }

        // GET: Cats
        public async Task<IActionResult> Index()
        {
              return _cats != null ? View(await(await _cats.FindAsync(_ => true)).ToListAsync()) : Problem("Entity set is null.");
        }

        // GET: Cats/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null || _cats == null)
            {
                return NotFound();
            }

            var cats = (await _cats.FindAsync(m => m.Id == id)).FirstOrDefault();

            if (cats == null)
            {
                return NotFound();
            }

            return View(cats);
        }

        // GET: Cats/Create
        public async Task<IActionResult> Create()
        {
            if (_cats == null)
            {
                return NotFound();
            }

            var cats = await (await _cats.FindAsync(_ => true)).ToListAsync();

            if (cats == null)
            {
                return NotFound();
            }

            var attrs = await (await _attrs.FindAsync(_ => true)).ToListAsync();

            if (attrs == null)
            {
                return NotFound();
            }

            MultiSelectList catNames = new MultiSelectList(cats.ConvertAll(c => c.Name));

            List<SelectListItem> attrNames = new List<SelectListItem>();
            attrs.ForEach(a => attrNames.Add(new SelectListItem(a.Name, a.Id)));

            ViewBag.Friends = catNames;
            ViewBag.Attributes = attrNames;
            return View();
        }

        // POST: Cats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Friends")] Cats cats, string? attributeId)
        {
            cats.CreatedDate = DateTime.Now;
            cats.UpdatedDate = DateTime.Now;

            var attrs = (await _attrs.FindAsync(m => m.Id == attributeId)).FirstOrDefault();
            cats.Attribute = attrs == null ? null : attrs;

            ModelState.ClearValidationState(nameof(cats));
            if (TryValidateModel(typeof(Cats), nameof(cats)))
            {
                await _cats.InsertOneAsync(cats);
                return RedirectToAction(nameof(Index));
            }
            return await Create();
        }

        // GET: Cats/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null || _cats == null)
            {
                return NotFound();
            }

            var cat = (await _cats.FindAsync(m => m.Id == id)).FirstOrDefault();
            if (cat == null)
            {
                return NotFound();
            }

            var cats = await (await _cats.FindAsync(_ => true)).ToListAsync();

            if (cats == null)
            {
                return NotFound();
            }

            var attrs = await (await _attrs.FindAsync(_ => true)).ToListAsync();

            if (attrs == null)
            {
                return NotFound();
            }

            MultiSelectList catNames = new MultiSelectList(cats.ConvertAll(c => c.Name));

            List<SelectListItem> attrNames = new List<SelectListItem>();
            attrs.ForEach(a => attrNames.Add(new SelectListItem(a.Name, a.Id)));

            ViewBag.Friends = catNames;
            ViewBag.Attributes = attrNames;
            return View(cat);
        }

        // POST: Cats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Description,Friends")] Cats cats, string? attributeId)
        {            
            if (id != cats.Id)
            {
                return NotFound();
            }

            var old_cats = (await _cats.FindAsync(m => m.Id == id)).FirstOrDefault();
            cats.CreatedDate = old_cats.CreatedDate;
            var attrs = (await _attrs.FindAsync(m => m.Id == attributeId)).FirstOrDefault();
            cats.Attribute = attrs == null ? null : attrs;

            ModelState.ClearValidationState(nameof(cats));
            if (TryValidateModel(typeof(Cats), nameof(cats)))
            {
                try
                {
                    await _cats.ReplaceOneAsync(m => m.Id == id, cats);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CatsExistsAsync(cats.Id).Result)
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
            return View(cats);
        }

        // GET: Cats/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null || _cats == null)
            {
                return NotFound();
            }

            var cats = (await _cats.FindAsync(m => m.Id == id)).FirstOrDefault();
            if (cats == null)
            {
                return NotFound();
            }

            return View(cats);
        }

        // POST: Cats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_cats == null)
            {
                return Problem("Entity set is null.");
            }
            var cats = (await _cats.FindAsync(m => m.Id == id)).FirstOrDefault();
            if (cats != null)
            {
                await _cats.DeleteOneAsync(x => x.Id == id);
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CatsExistsAsync(string id)
        {
          return (await _cats?.FindAsync(e => e.Id == id)!).Any();
        }

        //[Route("Default/IsCreditCardValid/{creditCardNumber}")]
        public async Task<IActionResult> QuerriesExample(int id)
        {
            //Find all friendly cats take name and attribute, limit to 5 and sort by name
            var q1 = _cats.
                Aggregate().
                Match(cats => cats.Friends.Count > 0).
                Limit(5).
                SortBy(cats => cats.Name).
                Project("{Name:1, Attribute:1}").
                ToList().ConvertAll(BsonTypeMapper.MapToDotNetValue);
            //Aggregate cats to all test ones, group by friendliness, add find avarage cuteness by group
            var q2 = _cats.
                Aggregate().
                Match(cats => cats.Description.Length == 1).
                Group(cats => cats.Friends.Count, cats => new { Friends = cats.Key, AvgCuteness = cats.Sum(cat => cat.Attribute.Cuteness) / cats.Count() }).
                SortBy(cats => cats.Friends).
                Project("{ Friends:1, AvgCuteness:1 }").
                ToList().ConvertAll(BsonTypeMapper.MapToDotNetValue);
            //Frendliest cat
            var sort = Builders<Cats>.Sort.Descending(cats => cats.Friends.Count);
            var q3 = _cats.
                Aggregate().
                Group(cats => cats, cats => new { Cat = cats.Key, Friend_Count = cats.Key.Friends.Count }).
                SortByDescending(cats => cats.Friend_Count).
                Limit(1).
                ToList().Select(cats => cats.Cat).ToList();
            //Cats per attribute
            var q4 = _cats.
                Aggregate().
                Group(cats => cats.Attribute, cats => new { Attribute = cats.Key, Cats = cats.Distinct(), Cats_Count = cats.Distinct().Count() }).
                SortByDescending(cats => cats.Cats_Count).
                Project("{Attribute:1, Cats:1}").
                ToList().ConvertAll(BsonTypeMapper.MapToDotNetValue);
            //Cats by cuteness
            var q5 = _cats.
                Aggregate().
                SortByDescending(cats => cats.Attribute.Cuteness).
                ToList();
            //TestOnesFilter
            var q6 = _cats.
                Aggregate().
                Match(cats => cats.Name.StartsWith("Test")).
                ToList();
            //Find Cats that's friends with x
            var q7querry = delegate (string x) { return _cats.
                Aggregate().
                Match(cats => cats.Friends.Contains(x)).
                ToList(); };
            var q7 = q7querry("Musya");
            //The avarage cats
            var q8 = _cats.
                Find(cats => cats.Attribute.Cuteness>=4 && cats.Attribute.Cuteness<=6).
                ToList();

            switch (id)
            {
                case 1: return Ok(q1);
                case 2: return Ok(q2);
                case 3: return Ok(q3);
                case 4: return Ok(q4);
                case 5: return Ok(q5);
                case 6: return Ok(q6);
                case 7: return Ok(q7);
                case 8: return Ok(q8);
                default: return Ok(q1.Concat(q2).Concat(q3).Concat(q4).Concat(q5).Concat(q6).Concat(q7).Concat(q8).ToList());
            }
        }
    }
}
