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
    public class FinesController : Controller
    {
        private readonly LibraryDbContext _context;

        public FinesController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Fines
        public async Task<IActionResult> Index(string searchQuery, string filterStatus, int page = 1)
        {
            var query = _context.Fines
                .Include(f => f.Student)
                .Include(f => f.BorrowRecord)
                    .ThenInclude(b => b.Book)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(f => 
                    f.Student.Name.Contains(searchQuery) ||
                    (f.BorrowRecord.Book != null && f.BorrowRecord.Book.Title.Contains(searchQuery))
                );
            }

            if (!string.IsNullOrEmpty(filterStatus) && filterStatus != "all")
            {
                query = query.Where(f => f.Status == filterStatus);
            }

            int pageSize = 5;
            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var paged = await query
                .OrderByDescending(f => f.GeneratedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var allFines = await _context.Fines.ToListAsync();

            var viewModel = new FineIndexViewModel
            {
                Fines = paged,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                SearchQuery = searchQuery,
                FilterStatus = filterStatus,
                TotalPendingFine = allFines.Where(f => f.Status == "Pending").Sum(f => f.Amount),
                TotalCollectedFine = allFines.Where(f => f.Status == "Paid").Sum(f => f.Amount)
            };

            return View(viewModel);
        }

        // GET: Fines/Create
        public IActionResult Create()
        {
            ViewData["BorrowId"] = new SelectList(_context.BorrowRecords.Include(b => b.Book), "Id", "Id");
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Name");
            return View();
        }

        // POST: Fines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Fine fine)
        {
            ModelState.Remove("FineId");
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(fine.Status)) fine.Status = "Pending";
                if (fine.GeneratedDate == default) fine.GeneratedDate = DateTime.Today;

                _context.Add(fine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BorrowId"] = new SelectList(_context.BorrowRecords, "Id", "Id", fine.BorrowId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Name", fine.StudentId);
            return View(fine);
        }

        // GET: Fines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var fine = await _context.Fines.FindAsync(id);
            if (fine == null) return NotFound();

            ViewData["BorrowId"] = new SelectList(_context.BorrowRecords, "Id", "Id", fine.BorrowId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Name", fine.StudentId);
            return View(fine);
        }

        // POST: Fines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Fine fine)
        {
            if (id != fine.FineId) return NotFound();

            ModelState.Remove("FineId");

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Fines.FindAsync(id);
                    if (existing == null) return NotFound();

                    existing.BorrowId = fine.BorrowId;
                    existing.StudentId = fine.StudentId;
                    existing.Amount = fine.Amount;
                    existing.Reason = fine.Reason;
                    existing.GeneratedDate = fine.GeneratedDate;
                    existing.PaidDate = fine.PaidDate;
                    existing.Status = fine.Status;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FineExists(fine.FineId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BorrowId"] = new SelectList(_context.BorrowRecords, "Id", "Id", fine.BorrowId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Name", fine.StudentId);
            return View(fine);
        }

        // POST: Fines/MarkPaid/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkPaid(int id)
        {
            var fine = await _context.Fines.FindAsync(id);
            if (fine != null && fine.Status != "Paid")
            {
                fine.Status = "Paid";
                fine.PaidDate = DateTime.Today;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Fines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var fine = await _context.Fines
                .Include(f => f.BorrowRecord)
                .Include(f => f.Student)
                .FirstOrDefaultAsync(m => m.FineId == id);
            if (fine == null) return NotFound();

            return View(fine);
        }

        // POST: Fines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fine = await _context.Fines.FindAsync(id);
            if (fine != null)
            {
                _context.Fines.Remove(fine);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool FineExists(int id)
        {
            return _context.Fines.Any(e => e.FineId == id);
        }
    }
}
