using System.Collections.Generic;
using LibraryManagement.MVC.Models;

namespace LibraryManagement.MVC.ViewModels
{
    public class NewspaperIndexViewModel
    {
        public List<Newspaper> Newspapers { get; set; } = new List<Newspaper>();
        
        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 5;

        // Search parameters
        public string SearchQuery { get; set; }
    }
}
