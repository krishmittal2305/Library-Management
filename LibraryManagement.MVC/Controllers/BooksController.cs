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
    public class BooksController : Controller
    {
        private readonly LibraryDbContext _context;

        public BooksController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index(string searchQuery, int page = 1)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(b => 
                    b.Title.Contains(searchQuery) || 
                    b.Author.Contains(searchQuery) || 
                    b.ISBN.Contains(searchQuery) ||
                    (b.Category != null && b.Category.Contains(searchQuery))
                );
            }

            // Pagination (5 records per page)
            int pageSize = 5;
            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var pagedBooks = await query
                .OrderBy(b => b.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new BookListViewModel
            {
                Books = pagedBooks,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                SearchQuery = searchQuery
            };

            return View(viewModel);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            ModelState.Remove("IsAvailable");
            ModelState.Remove("Status");
            
            if (!string.IsNullOrEmpty(book.ISBN) && _context.Books.Any(b => b.ISBN == book.ISBN))
            {
                ModelState.AddModelError("ISBN", "A book with this ISBN already exists.");
            }

            if (ModelState.IsValid)
            {
                book.IsAvailable = book.AvailableCopies > 0;
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book)
        {
            if (id != book.Id) return NotFound();

            ModelState.Remove("IsAvailable");
            ModelState.Remove("Status");

            if (!string.IsNullOrEmpty(book.ISBN) && _context.Books.Any(b => b.ISBN == book.ISBN && b.Id != id))
            {
                ModelState.AddModelError("ISBN", "A book with this ISBN already exists.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingBook = await _context.Books.FindAsync(id);
                    if (existingBook == null) return NotFound();

                    existingBook.Title = book.Title;
                    existingBook.Author = book.Author;
                    existingBook.ISBN = book.ISBN;
                    existingBook.Category = book.Category;
                    existingBook.TotalCopies = book.TotalCopies;
                    existingBook.AvailableCopies = book.AvailableCopies;
                    existingBook.IsAvailable = book.AvailableCopies > 0;
                    
                    // New Fields
                    existingBook.Publisher = book.Publisher;
                    existingBook.PublishedDate = book.PublishedDate;
                    existingBook.Description = book.Description;
                    existingBook.PageCount = book.PageCount;
                    existingBook.Language = book.Language;
                    existingBook.AverageRating = book.AverageRating;
                    existingBook.RatingsCount = book.RatingsCount;
                    existingBook.Thumbnail = book.Thumbnail;
                    existingBook.GoogleBookId = book.GoogleBookId;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (book == null) return NotFound();

            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (book == null) return NotFound();

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
