using System.Collections.Generic;

namespace LibraryManagement.MVC.ViewModels
{
    public class ReportViewModel<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public string SearchQuery { get; set; }
        
        public string ReportTitle { get; set; }
        public string ExportExcelAction { get; set; }
        public string ExportPdfAction { get; set; }
    }
}
