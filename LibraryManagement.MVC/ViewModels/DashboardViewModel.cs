using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.MVC.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalBooks { get; set; }
        public int AvailableBooks { get; set; }
        public int BorrowedBooks { get; set; }
        
        public int TotalStudents { get; set; }
        public int TotalLibrarians { get; set; }
        public int TotalMagazines { get; set; }
        public int TotalNewspapers { get; set; }
        public int TotalPublications { get; set; }

        public int TodaysBorrowings { get; set; }
        public int TodaysReturns { get; set; }
        
        public decimal TotalFine { get; set; }
        public decimal PendingFine { get; set; }
        public decimal CollectedFine { get; set; }

        public System.Collections.Generic.List<LibraryManagement.MVC.Models.BorrowRecord> RecentBorrows { get; set; } = new System.Collections.Generic.List<LibraryManagement.MVC.Models.BorrowRecord>();
        
        public System.Collections.Generic.List<LibraryManagement.MVC.Models.Book> FeaturedBooks { get; set; } = new System.Collections.Generic.List<LibraryManagement.MVC.Models.Book>();

        // For Chart.js
        public System.Collections.Generic.List<string> MonthlyLabels { get; set; } = new System.Collections.Generic.List<string>();
        public System.Collections.Generic.List<int> MonthlyBorrowCounts { get; set; } = new System.Collections.Generic.List<int>();

        public System.Collections.Generic.List<string> CategoryLabels { get; set; } = new System.Collections.Generic.List<string>();
        public System.Collections.Generic.List<int> CategoryCounts { get; set; } = new System.Collections.Generic.List<int>();
    }
}
