using System.Collections.Generic;
using LibraryManagement.MVC.Models;

namespace LibraryManagement.MVC.ViewModels
{
    public class FineIndexViewModel
    {
        public IEnumerable<Fine> Fines { get; set; }

        // Pagination
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 5;

        // Search parameters
        public string SearchQuery { get; set; }
        public string FilterStatus { get; set; }

        // Dashboard Stats
        public decimal TotalPendingFine { get; set; }
        public decimal TotalCollectedFine { get; set; }
    }
}
