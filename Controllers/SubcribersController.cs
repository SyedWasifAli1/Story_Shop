using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Book_kharido.Models;

namespace Book_kharido.Controllers
{
    public class SubcribersController : Controller
    {
        private readonly Project _context;

        public SubcribersController(Project context)
        {
            _context = context;
        }

        // GET: Subcribers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Subcribers.ToListAsync());
        }

        // GET: Subcribers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcriber = await _context.Subcribers
                .FirstOrDefaultAsync(m => m.SubId == id);
            if (subcriber == null)
            {
                return NotFound();
            }

            return View(subcriber);
        }

        // GET: Subcribers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subcribers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubId,SubName,Email")] Subcriber subcriber)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subcriber);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subcriber);
        }

        // GET: Subcribers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcriber = await _context.Subcribers.FindAsync(id);
            if (subcriber == null)
            {
                return NotFound();
            }
            return View(subcriber);
        }

        // POST: Subcribers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SubId,SubName,Email")] Subcriber subcriber)
        {
            if (id != subcriber.SubId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subcriber);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubcriberExists(subcriber.SubId))
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
            return View(subcriber);
        }

        // GET: Subcribers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcriber = await _context.Subcribers
                .FirstOrDefaultAsync(m => m.SubId == id);
            if (subcriber == null)
            {
                return NotFound();
            }

            return View(subcriber);
        }

        // POST: Subcribers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subcriber = await _context.Subcribers.FindAsync(id);
            if (subcriber != null)
            {
                _context.Subcribers.Remove(subcriber);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubcriberExists(int id)
        {
            return _context.Subcribers.Any(e => e.SubId == id);
        }
    }
}
