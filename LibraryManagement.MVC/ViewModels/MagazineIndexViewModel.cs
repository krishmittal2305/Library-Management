using System.Collections.Generic;
using LibraryManagement.MVC.Models;

namespace LibraryManagement.MVC.ViewModels
{
    public class MagazineIndexViewModel
    {
        public List<Magazine> Magazines { get; set; } = new List<Magazine>();
        
        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 5;

        // Search parameters
        public string SearchQuery { get; set; }
    }
}
