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
    public class NewspapersController : Controller
    {
        private readonly LibraryDbContext _context;

        public NewspapersController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Newspapers
        public async Task<IActionResult> Index(string searchQuery, int page = 1)
        {
            var query = _context.Newspapers.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(n => 
                    n.Title.Contains(searchQuery) || 
                    n.Publisher.Contains(searchQuery) ||
                    (n.Edition != null && n.Edition.Contains(searchQuery)) ||
                    n.Language.Contains(searchQuery)
                );
            }

            int pageSize = 5;
            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var paged = await query
                .OrderByDescending(n => n.PublishedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new NewspaperIndexViewModel
            {
                Newspapers = paged,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                SearchQuery = searchQuery
            };

            return View(viewModel);
        }

        // GET: Newspapers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Newspapers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Newspaper newspaper)
        {
            ModelState.Remove("NewspaperId");
            if (ModelState.IsValid)
            {
                _context.Add(newspaper);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(newspaper);
        }

        // GET: Newspapers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var newspaper = await _context.Newspapers.FindAsync(id);
            if (newspaper == null) return NotFound();

            return View(newspaper);
        }

        // POST: Newspapers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Newspaper newspaper)
        {
            if (id != newspaper.NewspaperId) return NotFound();

            ModelState.Remove("NewspaperId");

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Newspapers.FindAsync(id);
                    if (existing == null) return NotFound();

                    existing.Title = newspaper.Title;
                    existing.Publisher = newspaper.Publisher;
                    existing.PublishedDate = newspaper.PublishedDate;
                    existing.Edition = newspaper.Edition;
                    existing.Language = newspaper.Language;
                    existing.Availability = newspaper.Availability;
                    existing.Description = newspaper.Description;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewspaperExists(newspaper.NewspaperId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(newspaper);
        }

        // GET: Newspapers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var newspaper = await _context.Newspapers.FirstOrDefaultAsync(n => n.NewspaperId == id);
            if (newspaper == null) return NotFound();

            return View(newspaper);
        }

        // GET: Newspapers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var newspaper = await _context.Newspapers.FirstOrDefaultAsync(n => n.NewspaperId == id);
            if (newspaper == null) return NotFound();

            return View(newspaper);
        }

        // POST: Newspapers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var newspaper = await _context.Newspapers.FindAsync(id);
            if (newspaper != null)
            {
                _context.Newspapers.Remove(newspaper);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool NewspaperExists(int id)
        {
            return _context.Newspapers.Any(e => e.NewspaperId == id);
        }
    }
}
