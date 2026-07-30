using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using LibraryManagement.MVC.Data;
using LibraryManagement.MVC.ViewModels;
using LibraryManagement.MVC.Models;

namespace LibraryManagement.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly LibraryDbContext _context;
        public HomeController(LibraryDbContext context) { _context = context; }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(Dashboard));
            }

            var totalBooks = await _context.Books.CountAsync();
            var totalMagazines = await _context.Magazines.CountAsync();
            var totalNewspapers = await _context.Newspapers.CountAsync();
            var totalStudents = await _context.Students.CountAsync();
            var totalBorrows = await _context.BorrowRecords.CountAsync();

            ViewBag.TotalResources = totalBooks + totalMagazines + totalNewspapers;
            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalBorrows = totalBorrows;

            return View();
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var today = DateTime.Today;

            // Compute aggregates on the database instead of eager loading all records
            var totalBooks = await _context.Books.SumAsync(b => (int?)b.TotalCopies) ?? 0;
            var availableBooks = await _context.Books.SumAsync(b => (int?)b.AvailableCopies) ?? 0;
            var borrowedBooks = await _context.BorrowRecords.CountAsync(b => b.ReturnDate == null);
            var totalStudents = await _context.Students.CountAsync();
            var totalLibrarians = await _context.Librarians.CountAsync();
            var totalMagazines = await _context.Magazines.CountAsync();
            var totalNewspapers = await _context.Newspapers.CountAsync();
            var totalPublications = await _context.Publications.CountAsync();
            var todaysBorrowings = await _context.BorrowRecords.CountAsync(b => b.BorrowDate.Date == today);
            var todaysReturns = await _context.BorrowRecords.CountAsync(b => b.ReturnDate != null && b.ReturnDate.Value.Date == today);
            var totalFine = await _context.Fines.SumAsync(f => (decimal?)f.Amount) ?? 0;
            var collectedFine = await _context.Fines.Where(f => f.Status == "Paid").SumAsync(f => (decimal?)f.Amount) ?? 0;

            var recentBorrows = await _context.BorrowRecords
                .Include(b => b.Student)
                .Include(b => b.Book)
                .Include(b => b.Publication)
                .OrderByDescending(b => b.BorrowDate)
                .Take(5)
                .ToListAsync();

            var featuredBooks = await _context.Books
                .Where(b => b.IsAvailable)
                .OrderByDescending(b => b.Id)
                .Take(3)
                .ToListAsync();

            var vm = new DashboardViewModel
            {
                TotalBooks = totalBooks,
                AvailableBooks = availableBooks,
                BorrowedBooks = borrowedBooks,
                TotalStudents = totalStudents,
                TotalLibrarians = totalLibrarians,
                TotalMagazines = totalMagazines,
                TotalNewspapers = totalNewspapers,
                TotalPublications = totalPublications,
                TodaysBorrowings = todaysBorrowings,
                TodaysReturns = todaysReturns,
                RecentBorrows = recentBorrows,
                FeaturedBooks = featuredBooks,
                TotalFine = totalFine,
                CollectedFine = collectedFine,
                PendingFine = totalFine - collectedFine
            };

            // Generate Monthly Borrow Trend (Last 6 Months)
            var sixMonthsAgo = today.AddMonths(-5);
            var startDate = new DateTime(sixMonthsAgo.Year, sixMonthsAgo.Month, 1);
            
            var borrowCounts = await _context.BorrowRecords
                .Where(b => b.BorrowDate >= startDate)
                .GroupBy(b => new { b.BorrowDate.Year, b.BorrowDate.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync();

            for (int i = 5; i >= 0; i--)
            {
                var monthDate = today.AddMonths(-i);
                vm.MonthlyLabels.Add(monthDate.ToString("MMM yyyy"));
                var count = borrowCounts.FirstOrDefault(x => x.Year == monthDate.Year && x.Month == monthDate.Month)?.Count ?? 0;
                vm.MonthlyBorrowCounts.Add(count);
            }

            // Generate Books Category Doughnut Chart
            var categories = await _context.Books
                .GroupBy(b => string.IsNullOrWhiteSpace(b.Category) ? "Uncategorized" : b.Category)
                .Select(g => new { Key = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();
                                     
            foreach (var cat in categories)
            {
                vm.CategoryLabels.Add(cat.Key);
                vm.CategoryCounts.Add(cat.Count);
            }

            return View(vm);
        }
    }
}
