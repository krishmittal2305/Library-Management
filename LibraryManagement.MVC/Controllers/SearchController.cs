using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.MVC.Data;
using LibraryManagement.MVC.ViewModels;

namespace LibraryManagement.MVC.Controllers
{
    public class SearchController : Controller
    {
        private readonly LibraryDbContext _context;

        public SearchController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string query)
        {
            var viewModel = new GlobalSearchViewModel
            {
                Query = query
            };

            if (string.IsNullOrWhiteSpace(query))
            {
                return View(viewModel);
            }

            var lowerQuery = query.ToLower();

            // Search Books
            viewModel.Books = await _context.Books
                .Where(b => b.Title.ToLower().Contains(lowerQuery) || 
                            b.Author.ToLower().Contains(lowerQuery) || 
                            (b.ISBN != null && b.ISBN.Contains(lowerQuery)) ||
                            (b.Category != null && b.Category.ToLower().Contains(lowerQuery)) ||
                            (b.Publisher != null && b.Publisher.ToLower().Contains(lowerQuery)))
                .Take(20)
                .ToListAsync();

            // Search Publications
            viewModel.Publications = await _context.Publications
                .Where(p => p.Title.ToLower().Contains(lowerQuery) || 
                            p.Publisher.ToLower().Contains(lowerQuery))
                .Take(20)
                .ToListAsync();

            // Search Students
            viewModel.Students = await _context.Students
                .Where(s => s.Name.ToLower().Contains(lowerQuery) || 
                            s.EnrollmentNo.ToLower().Contains(lowerQuery) || 
                            (s.Email != null && s.Email.ToLower().Contains(lowerQuery)))
                .Take(20)
                .ToListAsync();

            // Search Librarians
            viewModel.Librarians = await _context.Librarians
                .Where(l => l.Name.ToLower().Contains(lowerQuery) || 
                            l.EmployeeId.ToLower().Contains(lowerQuery) || 
                            (l.Email != null && l.Email.ToLower().Contains(lowerQuery)))
                .Take(20)
                .ToListAsync();

            return View(viewModel);
        }
    }
}
