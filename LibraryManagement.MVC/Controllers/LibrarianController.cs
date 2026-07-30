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
    public class LibrarianController : Controller
    {
        private readonly LibraryDbContext _context;

        public LibrarianController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Librarian
        public async Task<IActionResult> Index(string searchQuery, int page = 1)
        {
            var query = _context.Librarians.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(l => 
                    l.Name.Contains(searchQuery) || 
                    l.EmployeeId.Contains(searchQuery) ||
                    (l.Email != null && l.Email.Contains(searchQuery)) ||
                    (l.Phone != null && l.Phone.Contains(searchQuery))
                );
            }

            int pageSize = 5;
            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var paged = await query
                .OrderBy(l => l.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new LibrarianIndexViewModel
            {
                Librarians = paged,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                SearchQuery = searchQuery
            };

            return View(viewModel);
        }

        // GET: Librarian/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Librarian/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Librarian librarian)
        {
            ModelState.Remove("Id");
            
            if (_context.Librarians.Any(l => l.EmployeeId == librarian.EmployeeId))
            {
                ModelState.AddModelError("EmployeeId", "A librarian with this Employee ID already exists.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(librarian);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(librarian);
        }

        // GET: Librarian/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var librarian = await _context.Librarians.FindAsync(id);
            if (librarian == null) return NotFound();
            return View(librarian);
        }

        // POST: Librarian/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Librarian librarian)
        {
            if (id != librarian.Id) return NotFound();
            ModelState.Remove("Id");
            
            if (_context.Librarians.Any(l => l.EmployeeId == librarian.EmployeeId && l.Id != id))
            {
                ModelState.AddModelError("EmployeeId", "A librarian with this Employee ID already exists.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Librarians.FindAsync(id);
                    if (existing == null) return NotFound();

                    existing.Name = librarian.Name;
                    existing.EmployeeId = librarian.EmployeeId;
                    existing.Email = librarian.Email;
                    existing.Phone = librarian.Phone;
                    existing.Shift = librarian.Shift;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibrarianExists(librarian.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(librarian);
        }

        // GET: Librarian/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var librarian = await _context.Librarians.FirstOrDefaultAsync(l => l.Id == id);
            if (librarian == null) return NotFound();
            return View(librarian);
        }

        // POST: Librarian/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var librarian = await _context.Librarians.FindAsync(id);
            if (librarian != null)
            {
                _context.Librarians.Remove(librarian);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Librarian/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var librarian = await _context.Librarians.FirstOrDefaultAsync(l => l.Id == id);
            if (librarian == null) return NotFound();
            return View(librarian);
        }

        private bool LibrarianExists(int id)
        {
            return _context.Librarians.Any(e => e.Id == id);
        }
    }
}
