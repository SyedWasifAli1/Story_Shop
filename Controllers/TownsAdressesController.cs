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
    public class TownsAdressesController : Controller
    {
        private readonly Project _context;

        public TownsAdressesController(Project context)
        {
            _context = context;
        }

        // GET: TownsAdresses
        public async Task<IActionResult> Index()
        {
            var project = _context.TownAddress.Include(t => t.City).Include(t => t.Country);
            return View(await project.ToListAsync());
        }

        // GET: TownsAdresses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var townsAdress = await _context.TownAddress
                .Include(t => t.City)
                .Include(t => t.Country)
                .FirstOrDefaultAsync(m => m.TownsAId == id);
            if (townsAdress == null)
            {
                return NotFound();
            }

            return View(townsAdress);
        }

        // GET: TownsAdresses/Create
        public IActionResult Create()
        {
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityId");
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "CountryId");
            return View();
        }

        // POST: TownsAdresses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TownsAId,TownName,KoloMeter,DeliveryFee,CityId,CountryId")] TownsAdress townsAdress)
        {
            if (ModelState.IsValid)
            {
                _context.Add(townsAdress);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityId", townsAdress.CityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "CountryId", townsAdress.CountryId);
            return View(townsAdress);
        }

        // GET: TownsAdresses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var townsAdress = await _context.TownAddress.FindAsync(id);
            if (townsAdress == null)
            {
                return NotFound();
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityId", townsAdress.CityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "CountryId", townsAdress.CountryId);
            return View(townsAdress);
        }

        // POST: TownsAdresses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TownsAId,TownName,KoloMeter,DeliveryFee,CityId,CountryId")] TownsAdress townsAdress)
        {
            if (id != townsAdress.TownsAId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(townsAdress);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TownsAdressExists(townsAdress.TownsAId))
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
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityId", townsAdress.CityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "CountryId", "CountryId", townsAdress.CountryId);
            return View(townsAdress);
        }

        // GET: TownsAdresses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var townsAdress = await _context.TownAddress
                .Include(t => t.City)
                .Include(t => t.Country)
                .FirstOrDefaultAsync(m => m.TownsAId == id);
            if (townsAdress == null)
            {
                return NotFound();
            }

            return View(townsAdress);
        }

        // POST: TownsAdresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var townsAdress = await _context.TownAddress.FindAsync(id);
            if (townsAdress != null)
            {
                _context.TownAddress.Remove(townsAdress);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TownsAdressExists(int id)
        {
            return _context.TownAddress.Any(e => e.TownsAId == id);
        }
    }
}
