using System.Collections.Generic;
using LibraryManagement.MVC.Models;

namespace LibraryManagement.MVC.ViewModels
{
    public class BookListViewModel
    {
        public IEnumerable<Book> Books { get; set; }

        // Pagination
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 5;

        // Search parameters
        public string SearchQuery { get; set; }
    }
}
