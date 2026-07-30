using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.MVC.Data;
using LibraryManagement.MVC.Models;
using LibraryManagement.MVC.ViewModels;

namespace LibraryManagement.MVC.Controllers
{
    public class BorrowController : Controller
    {
        private readonly LibraryDbContext _context;

        public BorrowController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Borrow
        public async Task<IActionResult> Index(string searchQuery, string filterStatus, int page = 1)
        {
            var query = _context.BorrowRecords
                .Include(b => b.Student)
                .Include(b => b.Book)
                .Include(b => b.Publication)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(b => 
                    b.Student.Name.Contains(searchQuery) ||
                    (b.Book != null && b.Book.Title.Contains(searchQuery)) ||
                    (b.Publication != null && b.Publication.Title.Contains(searchQuery))
                );
            }

            if (!string.IsNullOrEmpty(filterStatus) && filterStatus != "all")
            {
                if (filterStatus == "active")
                    query = query.Where(b => b.ReturnDate == null);
                else if (filterStatus == "returned")
                    query = query.Where(b => b.ReturnDate != null);
            }

            int pageSize = 5;
            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var paged = await query
                .OrderByDescending(b => b.BorrowDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new BorrowIndexViewModel
            {
                BorrowRecords = paged,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                SearchQuery = searchQuery,
                FilterStatus = filterStatus,
                LibrariansList = new SelectList(_context.Librarians, "Id", "Name")
            };

            return View(viewModel);
        }

        // GET: Borrow/Create
        public IActionResult Create(int? bookId = null)
        {
            var vm = new BorrowFormViewModel
            {
                BookId = bookId,
                StudentsList = new SelectList(_context.Students, "Id", "Name"),
                AvailableBooksList = new SelectList(_context.Books.Where(b => b.IsAvailable).OrderBy(b => b.Title).Take(500), "Id", "Title"),
                AvailablePublicationsList = new SelectList(_context.Publications.Where(p => p.IsAvailable), "Id", "Title"),
                LibrariansList = new SelectList(_context.Librarians, "Id", "Name"),
                DueDate = DateTime.Today.AddDays(15)
            };
            return View(vm);
        }

        // POST: Borrow/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BorrowFormViewModel model)
        {
            if (model.BookId == null && model.PublicationId == null)
            {
                ModelState.AddModelError("", "You must select either a Book or a Publication to borrow.");
            }
            if (model.BookId != null && model.PublicationId != null)
            {
                ModelState.AddModelError("", "You cannot select both a Book and a Publication in a single record.");
            }
            if (model.LibrarianId == null || model.LibrarianId == 0)
            {
                ModelState.AddModelError("LibrarianId", "Please select an issuing librarian.");
            }
            if (model.BorrowDate == default)
            {
                ModelState.AddModelError("BorrowDate", "Please enter a valid borrow date.");
            }
            if (model.DueDate == default || model.DueDate <= model.BorrowDate)
            {
                ModelState.AddModelError("DueDate", "Due date must be after the borrow date.");
            }

            if (ModelState.IsValid)
            {
                var record = new BorrowRecord
                {
                    StudentId = model.StudentId,
                    BookId = model.BookId,
                    PublicationId = model.PublicationId,
                    BorrowDate = model.BorrowDate,
                    DueDate = model.DueDate,
                    IssuedByLibrarianId = model.LibrarianId
                };

                if (model.BookId != null)
                {
                    var book = await _context.Books.FindAsync(model.BookId);
                    if (book != null)
                    {
                        if (book.AvailableCopies <= 0)
                        {
                            ModelState.AddModelError("BookId", "This book has no available copies.");
                            goto ValidationFailed;
                        }
                        book.AvailableCopies--;
                        book.IsAvailable = book.AvailableCopies > 0;
                    }
                }
                else if (model.PublicationId != null)
                {
                    var pub = await _context.Publications.FindAsync(model.PublicationId);
                    if (pub != null) pub.IsAvailable = false;
                }

                _context.BorrowRecords.Add(record);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Book issued successfully!";
                return RedirectToAction(nameof(Index));
            }

            ValidationFailed:
            model.StudentsList = new SelectList(_context.Students, "Id", "Name", model.StudentId);
            model.AvailableBooksList = new SelectList(_context.Books.Where(b => b.IsAvailable).OrderBy(b => b.Title).Take(500), "Id", "Title", model.BookId);
            model.AvailablePublicationsList = new SelectList(_context.Publications.Where(p => p.IsAvailable), "Id", "Title", model.PublicationId);
            model.LibrariansList = new SelectList(_context.Librarians, "Id", "Name", model.LibrarianId);
            return View(model);
        }

        // POST: Borrow/Return/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int id, int returnedByLibrarianId)
        {
            var record = await _context.BorrowRecords
                .Include(b => b.Book)
                .Include(b => b.Publication)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (record != null && record.ReturnDate == null)
            {
                record.ReturnDate = DateTime.Today;
                record.ReturnedByLibrarianId = returnedByLibrarianId;

                if (record.Book != null)
                {
                    record.Book.AvailableCopies++;
                    record.Book.IsAvailable = record.Book.AvailableCopies > 0;
                }
                else if (record.Publication != null)
                    record.Publication.IsAvailable = true;

                // Fines integration logic using DueDate
                if (DateTime.Today > record.DueDate)
                {
                    var diffDays = (DateTime.Today - record.DueDate).TotalDays;
                    var fineAmount = (decimal)(diffDays * 10);
                    _context.Fines.Add(new Fine
                    {
                        BorrowId = record.Id,
                        StudentId = record.StudentId,
                        Amount = fineAmount,
                        Reason = "Late Return",
                        GeneratedDate = DateTime.Today,
                        Status = "Pending"
                    });
                }

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
