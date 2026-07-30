using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using LibraryManagement.MVC.Models;

namespace LibraryManagement.MVC.ViewModels
{
    public class BorrowIndexViewModel
    {
        public IEnumerable<BorrowRecord> BorrowRecords { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 5;

        public string SearchQuery { get; set; }
        public string FilterStatus { get; set; }
        public SelectList LibrariansList { get; set; }
    }
}
