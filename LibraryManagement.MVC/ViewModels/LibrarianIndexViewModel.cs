using System.Collections.Generic;
using LibraryManagement.MVC.Models;

namespace LibraryManagement.MVC.ViewModels
{
    public class LibrarianIndexViewModel
    {
        public IEnumerable<Librarian> Librarians { get; set; }

        // Pagination
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 5;

        // Search parameters
        public string SearchQuery { get; set; }
    }
}
