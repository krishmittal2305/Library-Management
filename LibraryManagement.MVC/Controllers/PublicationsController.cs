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
    public class PublicationsController : Controller
    {
        private readonly LibraryDbContext _context;

        public PublicationsController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Publications
        // Supports filtering by type (/Publications?type=Magazine)
        public async Task<IActionResult> Index(PublicationType? type, string searchQuery, int page = 1)
        {
            var query = _context.Publications.AsQueryable();

            if (type.HasValue)
            {
                query = query.Where(p => p.Type == type.Value);
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(p => 
                    p.Title.Contains(searchQuery) || 
                    (p.Publisher != null && p.Publisher.Contains(searchQuery))
                );
            }

            int pageSize = 5;
            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var paged = await query
                .OrderByDescending(p => p.PublishedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new PublicationIndexViewModel
            {
                Publications = paged,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                SearchQuery = searchQuery,
                FilterType = type
            };

            ViewData["Title"] = type.HasValue ? type.Value.ToString() + "s" : "Publications";
            ViewData["CurrentType"] = type;

            return View(viewModel);
        }

        // GET: Publications/Create
        public IActionResult Create(PublicationType? type)
        {
            var model = new Publication();
            if (type.HasValue)
            {
                model.Type = type.Value;
            }
            return View(model);
        }

        // POST: Publications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Publication publication)
        {
            ModelState.Remove("Id");
            ModelState.Remove("IsAvailable");
            if (ModelState.IsValid)
            {
                _context.Add(publication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { type = publication.Type });
            }
            return View(publication);
        }

        // GET: Publications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var publication = await _context.Publications.FindAsync(id);
            if (publication == null) return NotFound();

            return View(publication);
        }

        // POST: Publications/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Publication publication)
        {
            if (id != publication.Id) return NotFound();

            ModelState.Remove("Id");
            ModelState.Remove("IsAvailable");

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Publications.FindAsync(id);
                    if (existing == null) return NotFound();

                    existing.Title = publication.Title;
                    existing.Publisher = publication.Publisher;
                    existing.PublishedDate = publication.PublishedDate;
                    existing.Type = publication.Type;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PublicationExists(publication.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index), new { type = publication.Type });
            }
            return View(publication);
        }

        // GET: Publications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var publication = await _context.Publications.FirstOrDefaultAsync(m => m.Id == id);
            if (publication == null) return NotFound();

            return View(publication);
        }

        // POST: Publications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var publication = await _context.Publications.FindAsync(id);
            if (publication != null)
            {
                _context.Publications.Remove(publication);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { type = publication?.Type });
        }

        private bool PublicationExists(int id)
        {
            return _context.Publications.Any(e => e.Id == id);
        }
    }
}
