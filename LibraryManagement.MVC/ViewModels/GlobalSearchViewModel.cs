using System.Collections.Generic;
using LibraryManagement.MVC.Models;

namespace LibraryManagement.MVC.ViewModels
{
    public class GlobalSearchViewModel
    {
        public string Query { get; set; }
        
        public List<Book> Books { get; set; } = new List<Book>();
        public List<Publication> Publications { get; set; } = new List<Publication>();
        public List<Student> Students { get; set; } = new List<Student>();
        public List<Librarian> Librarians { get; set; } = new List<Librarian>();
    }
}
