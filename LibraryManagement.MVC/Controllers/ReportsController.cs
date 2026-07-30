using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.MVC.Data;
using LibraryManagement.MVC.ViewModels;

namespace LibraryManagement.MVC.Controllers
{
    public class ReportsController : Controller
    {
        private readonly LibraryDbContext _context;

        public ReportsController(LibraryDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Books Report
        public async Task<IActionResult> Books(string searchQuery, int page = 1)
        {
            var query = _context.Books.AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(b => b.Title.Contains(searchQuery) || b.Author.Contains(searchQuery) || b.ISBN.Contains(searchQuery));
            }

            int pageSize = 10;
            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            if (page < 1) page = 1; if (page > totalPages && totalPages > 0) page = totalPages;

            var paged = await query.OrderBy(b => b.Title).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return View(new ReportViewModel<LibraryManagement.MVC.Models.Book>
            {
                Items = paged, CurrentPage = page, TotalPages = totalPages, PageSize = pageSize, SearchQuery = searchQuery,
                ReportTitle = "Books Inventory Report", ExportExcelAction = "ExportBooksExcel", ExportPdfAction = "ExportBooksPdf"
            });
        }

        public async Task<IActionResult> ExportBooksExcel(string searchQuery)
        {
            var query = _context.Books.AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery)) query = query.Where(b => b.Title.Contains(searchQuery) || b.Author.Contains(searchQuery));
            var data = await query.OrderBy(b => b.Title).ToListAsync();

            var builder = new StringBuilder();
            builder.AppendLine("ID,Title,Author,ISBN,Category,Publisher,Language,Pages,Published Date,Total Copies,Available");
            foreach (var item in data) 
            {
                builder.AppendLine($"{item.Id},\"{item.Title}\",\"{item.Author}\",\"{item.ISBN}\",\"{item.Category}\",\"{item.Publisher}\",\"{item.Language}\",{item.PageCount},{(item.PublishedDate.HasValue ? item.PublishedDate.Value.ToString("yyyy-MM-dd") : "")},{item.TotalCopies},{item.AvailableCopies}");
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "BooksReport.csv");
        }

        public async Task<IActionResult> ExportBooksPdf(string searchQuery)
        {
            var query = _context.Books.AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery)) query = query.Where(b => b.Title.Contains(searchQuery) || b.Author.Contains(searchQuery));
            var data = await query.OrderBy(b => b.Title).ToListAsync();
            ViewData["Title"] = "Books Inventory Report";
            return View("BooksPrint", data);
        }
        #endregion

        #region Students Report
        public async Task<IActionResult> Students(string searchQuery, int page = 1)
        {
            var query = _context.Students.AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(s => s.Name.Contains(searchQuery) || s.EnrollmentNo.Contains(searchQuery));
            }

            int pageSize = 10;
            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            if (page < 1) page = 1; if (page > totalPages && totalPages > 0) page = totalPages;

            var paged = await query.OrderBy(s => s.Name).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return View(new ReportViewModel<LibraryManagement.MVC.Models.Student>
            {
                Items = paged, CurrentPage = page, TotalPages = totalPages, PageSize = pageSize, SearchQuery = searchQuery,
                ReportTitle = "Students Report", ExportExcelAction = "ExportStudentsExcel", ExportPdfAction = "ExportStudentsPdf"
            });
        }

        public async Task<IActionResult> ExportStudentsExcel(string searchQuery)
        {
            var query = _context.Students.AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery)) query = query.Where(s => s.Name.Contains(searchQuery) || s.EnrollmentNo.Contains(searchQuery));
            var data = await query.OrderBy(s => s.Name).ToListAsync();

            var builder = new StringBuilder();
            builder.AppendLine("ID,Enrollment No,Name,Email,Phone,Department,Semester");
            foreach (var item in data) builder.AppendLine($"{item.Id},{item.EnrollmentNo},\"{item.Name}\",{item.Email},{item.Phone},\"{item.Department}\",{item.Semester}");

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "StudentsReport.csv");
        }

        public async Task<IActionResult> ExportStudentsPdf(string searchQuery)
        {
            var query = _context.Students.AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery)) query = query.Where(s => s.Name.Contains(searchQuery) || s.EnrollmentNo.Contains(searchQuery));
            var data = await query.OrderBy(s => s.Name).ToListAsync();
            ViewData["Title"] = "Students Report";
            return View("StudentsPrint", data);
        }
        #endregion

        #region Librarians Report
        public async Task<IActionResult> Librarians(string searchQuery, int page = 1)
        {
            var query = _context.Librarians.AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(l => l.Name.Contains(searchQuery) || l.EmployeeId.Contains(searchQuery));
            }

            int pageSize = 10;
            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            if (page < 1) page = 1; if (page > totalPages && totalPages > 0) page = totalPages;

            var paged = await query.OrderBy(l => l.Name).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return View(new ReportViewModel<LibraryManagement.MVC.Models.Librarian>
            {
                Items = paged, CurrentPage = page, TotalPages = totalPages, PageSize = pageSize, SearchQuery = searchQuery,
                ReportTitle = "Librarians Report", ExportExcelAction = "ExportLibrariansExcel", ExportPdfAction = "ExportLibrariansPdf"
            });
        }

        public async Task<IActionResult> ExportLibrariansExcel(string searchQuery)
        {
            var query = _context.Librarians.AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery)) query = query.Where(l => l.Name.Contains(searchQuery) || l.EmployeeId.Contains(searchQuery));
            var data = await query.OrderBy(l => l.Name).ToListAsync();

            var builder = new StringBuilder();
            builder.AppendLine("ID,Employee ID,Name,Email,Phone,Shift");
            foreach (var item in data) builder.AppendLine($"{item.Id},{item.EmployeeId},\"{item.Name}\",{item.Email},{item.Phone},{item.Shift}");

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "LibrariansReport.csv");
        }

        public async Task<IActionResult> ExportLibrariansPdf(string searchQuery)
        {
            var query = _context.Librarians.AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery)) query = query.Where(l => l.Name.Contains(searchQuery) || l.EmployeeId.Contains(searchQuery));
            var data = await query.OrderBy(l => l.Name).ToListAsync();
            ViewData["Title"] = "Librarians Report";
            return View("LibrariansPrint", data);
        }
        #endregion

        #region Borrow History Report
        public async Task<IActionResult> BorrowHistory(string searchQuery, int page = 1)
        {
            var query = _context.BorrowRecords.Include(b => b.Student).Include(b => b.Book).AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(b => b.Student.Name.Contains(searchQuery) || (b.Book != null && b.Book.Title.Contains(searchQuery)));
            }

            int pageSize = 10;
            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            if (page < 1) page = 1; if (page > totalPages && totalPages > 0) page = totalPages;

            var paged = await query.OrderByDescending(b => b.BorrowDate).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return View(new ReportViewModel<LibraryManagement.MVC.Models.BorrowRecord>
            {
                Items = paged, CurrentPage = page, TotalPages = totalPages, PageSize = pageSize, SearchQuery = searchQuery,
                ReportTitle = "Borrow History Report", ExportExcelAction = "ExportBorrowHistoryExcel", ExportPdfAction = "ExportBorrowHistoryPdf"
            });
        }

        public async Task<IActionResult> ExportBorrowHistoryExcel(string searchQuery)
        {
            var query = _context.BorrowRecords.Include(b => b.Student).Include(b => b.Book).AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery)) query = query.Where(b => b.Student.Name.Contains(searchQuery) || (b.Book != null && b.Book.Title.Contains(searchQuery)));
            var data = await query.OrderByDescending(b => b.BorrowDate).ToListAsync();

            var builder = new StringBuilder();
            builder.AppendLine("ID,Student,Book,Borrow Date,Due Date,Return Date");
            foreach (var item in data) builder.AppendLine($"{item.Id},\"{item.Student?.Name}\",\"{item.Book?.Title}\",{item.BorrowDate:yyyy-MM-dd},{item.DueDate:yyyy-MM-dd},{(item.ReturnDate.HasValue ? item.ReturnDate.Value.ToString("yyyy-MM-dd") : "Not Returned")}");

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "BorrowHistoryReport.csv");
        }

        public async Task<IActionResult> ExportBorrowHistoryPdf(string searchQuery)
        {
            var query = _context.BorrowRecords.Include(b => b.Student).Include(b => b.Book).AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery)) query = query.Where(b => b.Student.Name.Contains(searchQuery) || (b.Book != null && b.Book.Title.Contains(searchQuery)));
            var data = await query.OrderByDescending(b => b.BorrowDate).ToListAsync();
            ViewData["Title"] = "Borrow History Report";
            return View("BorrowHistoryPrint", data);
        }
        #endregion

        #region Fines Report
        public async Task<IActionResult> Fines(string searchQuery, int page = 1)
        {
            var query = _context.Fines.Include(f => f.Student).AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(f => f.Student.Name.Contains(searchQuery));
            }

            int pageSize = 10;
            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            if (page < 1) page = 1; if (page > totalPages && totalPages > 0) page = totalPages;

            var paged = await query.OrderByDescending(f => f.GeneratedDate).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return View(new ReportViewModel<LibraryManagement.MVC.Models.Fine>
            {
                Items = paged, CurrentPage = page, TotalPages = totalPages, PageSize = pageSize, SearchQuery = searchQuery,
                ReportTitle = "Fine Report", ExportExcelAction = "ExportFinesExcel", ExportPdfAction = "ExportFinesPdf"
            });
        }

        public async Task<IActionResult> ExportFinesExcel(string searchQuery)
        {
            var query = _context.Fines.Include(f => f.Student).AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery)) query = query.Where(f => f.Student.Name.Contains(searchQuery));
            var data = await query.OrderByDescending(f => f.GeneratedDate).ToListAsync();

            var builder = new StringBuilder();
            builder.AppendLine("Fine ID,Student,Borrow ID,Amount,Reason,Generated Date,Paid Date,Status");
            foreach (var item in data) builder.AppendLine($"{item.FineId},\"{item.Student?.Name}\",{item.BorrowId},{item.Amount},\"{item.Reason}\",{item.GeneratedDate:yyyy-MM-dd},{(item.PaidDate.HasValue ? item.PaidDate.Value.ToString("yyyy-MM-dd") : "")},\"{item.Status}\"");

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "FinesReport.csv");
        }

        public async Task<IActionResult> ExportFinesPdf(string searchQuery)
        {
            var query = _context.Fines.Include(f => f.Student).AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery)) query = query.Where(f => f.Student.Name.Contains(searchQuery));
            var data = await query.OrderByDescending(f => f.GeneratedDate).ToListAsync();
            ViewData["Title"] = "Fines Report";
            return View("FinesPrint", data);
        }
        #endregion

        #region Magazines Report
        public async Task<IActionResult> Magazines(string searchQuery, int page = 1)
        {
            var query = _context.Magazines.AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(m => m.Title.Contains(searchQuery) || m.Publisher.Contains(searchQuery));
            }

            int pageSize = 10;
            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            if (page < 1) page = 1; if (page > totalPages && totalPages > 0) page = totalPages;

            var paged = await query.OrderBy(m => m.Title).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return View(new ReportViewModel<LibraryManagement.MVC.Models.Magazine>
            {
                Items = paged, CurrentPage = page, TotalPages = totalPages, PageSize = pageSize, SearchQuery = searchQuery,
                ReportTitle = "Magazines Report", ExportExcelAction = "ExportMagazinesExcel", ExportPdfAction = "ExportMagazinesPdf"
            });
        }

        public async Task<IActionResult> ExportMagazinesExcel(string searchQuery)
        {
            var query = _context.Magazines.AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery)) query = query.Where(m => m.Title.Contains(searchQuery) || m.Publisher.Contains(searchQuery));
            var data = await query.OrderBy(m => m.Title).ToListAsync();

            var builder = new StringBuilder();
            builder.AppendLine("ID,Title,Publisher,Category,Language,Published Date,Available");
            foreach (var item in data) builder.AppendLine($"{item.MagazineId},\"{item.Title}\",\"{item.Publisher}\",\"{item.Category}\",\"{item.Language}\",{item.PublishedDate:yyyy-MM-dd},{item.Availability}");

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "MagazinesReport.csv");
        }

        public async Task<IActionResult> ExportMagazinesPdf(string searchQuery)
        {
            var query = _context.Magazines.AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery)) query = query.Where(m => m.Title.Contains(searchQuery) || m.Publisher.Contains(searchQuery));
            var data = await query.OrderBy(m => m.Title).ToListAsync();
            ViewData["Title"] = "Magazines Report";
            return View("MagazinesPrint", data);
        }
        #endregion

        #region Newspapers Report
        public async Task<IActionResult> Newspapers(string searchQuery, int page = 1)
        {
            var query = _context.Newspapers.AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(n => n.Title.Contains(searchQuery) || n.Publisher.Contains(searchQuery));
            }

            int pageSize = 10;
            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            if (page < 1) page = 1; if (page > totalPages && totalPages > 0) page = totalPages;

            var paged = await query.OrderBy(n => n.Title).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return View(new ReportViewModel<LibraryManagement.MVC.Models.Newspaper>
            {
                Items = paged, CurrentPage = page, TotalPages = totalPages, PageSize = pageSize, SearchQuery = searchQuery,
                ReportTitle = "Newspapers Report", ExportExcelAction = "ExportNewspapersExcel", ExportPdfAction = "ExportNewspapersPdf"
            });
        }

        public async Task<IActionResult> ExportNewspapersExcel(string searchQuery)
        {
            var query = _context.Newspapers.AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery)) query = query.Where(n => n.Title.Contains(searchQuery) || n.Publisher.Contains(searchQuery));
            var data = await query.OrderBy(n => n.Title).ToListAsync();

            var builder = new StringBuilder();
            builder.AppendLine("ID,Title,Publisher,Edition,Language,Published Date,Available");
            foreach (var item in data) builder.AppendLine($"{item.NewspaperId},\"{item.Title}\",\"{item.Publisher}\",\"{item.Edition}\",\"{item.Language}\",{item.PublishedDate:yyyy-MM-dd},{item.Availability}");

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "NewspapersReport.csv");
        }

        public async Task<IActionResult> ExportNewspapersPdf(string searchQuery)
        {
            var query = _context.Newspapers.AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery)) query = query.Where(n => n.Title.Contains(searchQuery) || n.Publisher.Contains(searchQuery));
            var data = await query.OrderBy(n => n.Title).ToListAsync();
            ViewData["Title"] = "Newspapers Report";
            return View("NewspapersPrint", data);
        }
        #endregion
    }
}
