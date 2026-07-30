using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.MVC.Data;
using LibraryManagement.MVC.Models;
using LibraryManagement.MVC.ViewModels;

namespace LibraryManagement.MVC.Controllers
{
    public class MagazinesController : Controller
    {
        private readonly LibraryDbContext _context;

        public MagazinesController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Magazines
        public async Task<IActionResult> Index(string searchQuery, int page = 1)
        {
            var query = _context.Magazines.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(m => 
                    m.Title.Contains(searchQuery) || 
                    m.Publisher.Contains(searchQuery) ||
                    m.Language.Contains(searchQuery) ||
                    m.Category.Contains(searchQuery)
                );
            }

            int pageSize = 5;
            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var paged = await query
                .OrderByDescending(m => m.PublishedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new MagazineIndexViewModel
            {
                Magazines = paged,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                SearchQuery = searchQuery
            };

            return View(viewModel);
        }

        // GET: Magazines/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Magazines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Magazine magazine)
        {
            ModelState.Remove("MagazineId");
            if (ModelState.IsValid)
            {
                _context.Add(magazine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(magazine);
        }

        // GET: Magazines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var magazine = await _context.Magazines.FindAsync(id);
            if (magazine == null) return NotFound();

            return View(magazine);
        }

        // POST: Magazines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Magazine magazine)
        {
            if (id != magazine.MagazineId) return NotFound();

            ModelState.Remove("MagazineId");

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Magazines.FindAsync(id);
                    if (existing == null) return NotFound();

                    existing.Title = magazine.Title;
                    existing.Publisher = magazine.Publisher;
                    existing.PublishedDate = magazine.PublishedDate;
                    existing.Language = magazine.Language;
                    existing.Category = magazine.Category;
                    existing.Availability = magazine.Availability;
                    existing.Description = magazine.Description;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MagazineExists(magazine.MagazineId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(magazine);
        }

        // GET: Magazines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var magazine = await _context.Magazines.FirstOrDefaultAsync(m => m.MagazineId == id);
            if (magazine == null) return NotFound();

            return View(magazine);
        }

        // GET: Magazines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var magazine = await _context.Magazines.FirstOrDefaultAsync(m => m.MagazineId == id);
            if (magazine == null) return NotFound();

            return View(magazine);
        }

        // POST: Magazines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var magazine = await _context.Magazines.FindAsync(id);
            if (magazine != null)
            {
                _context.Magazines.Remove(magazine);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MagazineExists(int id)
        {
            return _context.Magazines.Any(e => e.MagazineId == id);
        }
    }
}
