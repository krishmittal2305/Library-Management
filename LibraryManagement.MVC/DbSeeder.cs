using System;
using System.Collections.Generic;
using System.Linq;
using LibraryManagement.MVC.Data;
using LibraryManagement.MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.MVC
{
    public static class DbSeeder
    {
        public static void Seed(LibraryDbContext context)
        {
            context.Database.Migrate();

            // MIGRATION: Move generated Magazines and Newspapers to their respective tables
            if (!context.Magazines.Any() && context.Publications.Any(p => p.Type == PublicationType.Magazine))
            {
                var mags = context.Publications.Where(p => p.Type == PublicationType.Magazine).ToList();
                foreach (var p in mags)
                {
                    context.Magazines.Add(new Magazine { Title = p.Title, Publisher = p.Publisher, PublishedDate = p.PublishedDate, Language = "English", Category = "Technology", Availability = p.IsAvailable, Description = $"Magazine issue of {p.Title}" });
                }
                var news = context.Publications.Where(p => p.Type == PublicationType.Newspaper).ToList();
                foreach (var p in news)
                {
                    context.Newspapers.Add(new Newspaper { Title = p.Title, Publisher = p.Publisher, PublishedDate = p.PublishedDate, Language = "English", Availability = p.IsAvailable, Edition = "Morning", Description = $"Newspaper edition of {p.Title}" });
                }
                context.Publications.RemoveRange(mags);
                context.Publications.RemoveRange(news);
                context.SaveChanges();
            }

            // Prevent reseeding if data already exists
            if (context.Books.Any())
            {
                return;
            }

            // Clear all data cleanly with CASCADE
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE \"Fines\", \"BorrowRecords\", \"Books\", \"Students\", \"Librarians\", \"Publications\" CASCADE;");

            var random = new Random(12345); // deterministic seed for reproducibility

            // GENERATE LIBRARIANS (20)
            var librarianNames = new[] { 
                "Aarav Sharma", "Vivaan Patel", "Aditya Kumar", "Vihaan Singh", 
                "Arjun Gupta", "Sai Verma", "Ayaan Reddy", "Krishna Rao",
                "Ishaan Desai", "Shaurya Joshi", "Diya Iyer", "Saanvi Nair",
                "Aanya Menon", "Myra Pillai", "Ananya Das", "Pari Bose",
                "Aarohi Sen", "Avni Chatterjee", "Kavya Banerjee", "Navya Mukherjee"
            };

            var librarians = new List<Librarian>();
            for (int i = 0; i < 20; i++)
            {
                librarians.Add(new Librarian
                {
                    Name = librarianNames[i],
                    EmployeeId = $"LIB{i+1:000}",
                    Email = $"{librarianNames[i].Split(' ')[0].ToLower()}.{librarianNames[i].Split(' ')[1].ToLower()}@library.mponline.gov.in",
                    Phone = $"9{random.Next(100000000, 999999999)}",
                    Shift = i % 2 == 0 ? "Morning" : "Evening"
                });
            }
            context.Librarians.AddRange(librarians);

            // GENERATE STUDENTS (100)
            var firstNames = new[] { "Rahul", "Amit", "Vikram", "Sneha", "Priya", "Neha", "Rohit", "Anjali", "Suresh", "Kiran", 
                                     "Sunil", "Rakesh", "Anil", "Deepa", "Pooja", "Ritu", "Manoj", "Sanjay", "Anita", "Geeta",
                                     "Rahul", "Ravi", "Mohan", "Meena", "Seema", "Arun", "Kavita", "Sita", "Gita", "Ram" };
            var lastNames = new[] { "Sharma", "Patel", "Kumar", "Singh", "Gupta", "Verma", "Reddy", "Rao", "Desai", "Joshi",
                                    "Iyer", "Nair", "Menon", "Pillai", "Das", "Bose", "Sen", "Chatterjee", "Banerjee", "Mukherjee",
                                    "Yadav", "Chauhan", "Choudhary", "Bhat", "Bhattacharya", "Agarwal", "Mishra", "Pandey", "Dixit", "Dubey" };
            var departments = new[] { "Computer Science", "Information Technology", "Electronics", "Mechanical", "Civil", 
                                      "Electrical", "MBA", "BBA", "BCA", "MCA", "Artificial Intelligence", "Data Science" };

            var students = new List<Student>();
            for (int i = 0; i < 100; i++)
            {
                var fn = firstNames[random.Next(firstNames.Length)];
                var ln = lastNames[random.Next(lastNames.Length)];
                students.Add(new Student
                {
                    EnrollmentNo = $"ENR{2024}{i+1:000}",
                    Name = $"{fn} {ln}",
                    Email = $"{fn.ToLower()}.{ln.ToLower()}{i+1}@university.edu.in",
                    Phone = $"8{random.Next(10000000, 99999999)}0",
                    Department = departments[random.Next(departments.Length)],
                    Semester = random.Next(1, 9)
                });
            }
            context.Students.AddRange(students);

            // GENERATE MAGAZINES (20)
            var magazineTitles = new[] { "IEEE Spectrum", "National Geographic", "Scientific American", "Nature", "TIME", "Forbes", "Fortune", "PC Magazine", "Linux Journal", "Wired", "MIT Technology Review", "India Today" };
            var publications = new List<Publication>();
            for (int i = 0; i < 20; i++)
            {
                var title = magazineTitles[random.Next(magazineTitles.Length)];
                publications.Add(new Publication
                {
                    Title = $"{title} - Issue {i+1}",
                    Publisher = "Various",
                    PublishedDate = DateTime.Today.AddDays(-random.Next(1, 700)),
                    Type = PublicationType.Magazine,
                    IsAvailable = true
                });
            }
            
            // GENERATE NEWSPAPERS (10)
            var newspaperTitles = new[] { "The Hindu", "Times of India", "Indian Express", "Economic Times", "Business Standard", "The Telegraph", "Hindustan Times", "Dainik Bhaskar", "The Pioneer", "Mint" };
            for (int i = 0; i < 10; i++)
            {
                var title = newspaperTitles[random.Next(newspaperTitles.Length)];
                publications.Add(new Publication
                {
                    Title = $"{title} - Edition {i+1}",
                    Publisher = "Daily News",
                    PublishedDate = DateTime.Today.AddDays(-random.Next(1, 100)),
                    Type = PublicationType.Newspaper,
                    IsAvailable = true
                });
            }
            context.Publications.AddRange(publications);

            // GENERATE BOOKS (5000)
            var adjectives = new[] { "Advanced", "Introduction to", "Mastering", "Fundamentals of", "Applied", "Practical", "Modern", "Essential", "Principles of", "The Art of", "Foundations of", "Understanding", "Designing", "Developing", "Implementing", "Exploring", "A Guide to", "Handbook of", "Concepts in", "Trends in", "Discovering", "Learning", "Professional", "Beginning", "Comprehensive" };
            var topics = new[] { "Artificial Intelligence", "Machine Learning", "Data Science", "Software Engineering", "Programming", "Cyber Security", "Networking", "Databases", "Cloud Computing", "Mathematics", "Physics", "Chemistry", "Biology", "Economics", "Business", "Finance", "Marketing", "History", "Geography", "Literature", "Philosophy", "Psychology", "Civil Engineering", "Mechanical Engineering", "Electronics", "Electrical Engineering", "Deep Learning", "Algorithms", "Data Structures", "Web Development", "Mobile Development", "Blockchain", "IoT", "Robotics", "Quantum Computing", "Game Design", "Operating Systems", "Computer Graphics", "Bioinformatics", "E-commerce", "C#", "Java", "Python", "C++", "JavaScript", "TypeScript", "React", "Angular", "Vue" };
            var suffixes = new[] { "with Python", "for Beginners", "in Practice", "A Modern Approach", "and Applications", "for Professionals", "in the 21st Century", "Theory and Practice", "A Comprehensive Guide", "Step by Step", "for Students", "Explained", "Demystified", "", "", "", "", "", "", "" };
            var publishers = new[] { "O'Reilly Media", "Pearson", "McGraw Hill", "Springer", "Wiley", "Manning Publications", "Packt Publishing", "Addison-Wesley", "Cambridge University Press", "Oxford University Press" };

            var books = new List<Book>();
            var uniqueTitles = new HashSet<string>();

            int bookCount = 0;
            while(bookCount < 5000)
            {
                var adj = adjectives[random.Next(adjectives.Length)];
                var topic = topics[random.Next(topics.Length)];
                var suffix = suffixes[random.Next(suffixes.Length)];
                var title = $"{adj} {topic} {suffix}".Trim().Replace("  ", " ");
                
                if (!uniqueTitles.Contains(title))
                {
                    uniqueTitles.Add(title);
                    
                    var fn = firstNames[random.Next(firstNames.Length)];
                    var ln = lastNames[random.Next(lastNames.Length)];
                    
                    var totalCopies = random.Next(1, 15);
                    
                    books.Add(new Book
                    {
                        Title = title,
                        Author = $"{fn} {ln}",
                        ISBN = $"978-{random.Next(10, 99)}-{random.Next(1000, 9999)}-{random.Next(100, 999)}-{random.Next(0, 9)}",
                        Category = topic,
                        TotalCopies = totalCopies,
                        AvailableCopies = totalCopies,
                        IsAvailable = true,
                        Publisher = publishers[random.Next(publishers.Length)],
                        PublishedDate = DateTime.Today.AddDays(-random.Next(100, 5000)),
                        Description = $"A comprehensive book on {topic}.",
                        PageCount = random.Next(150, 1000),
                        Language = "English"
                    });
                    bookCount++;
                }
            }
            context.Books.AddRange(books);
            
            // Save all the base records first so we get IDs
            context.SaveChanges();

            // GENERATE BORROW RECORDS (~800)
            var borrowRecords = new List<BorrowRecord>();
            var fines = new List<Fine>();
            
            int pendingFinesCount = 0;
            int paidFinesCount = 0;
            
            for (int i = 0; i < 800; i++)
            {
                var student = students[random.Next(students.Count)];
                var book = books[random.Next(books.Count)];
                var libIssue = librarians[random.Next(librarians.Count)];
                
                // Only borrow if copies available
                if (book.AvailableCopies > 0)
                {
                    book.AvailableCopies--;
                    book.IsAvailable = book.AvailableCopies > 0;
                    
                    var borrowDaysAgo = random.Next(1, 700);
                    var borrowDate = DateTime.Today.AddDays(-borrowDaysAgo);
                    var dueDate = borrowDate.AddDays(15);
                    
                    DateTime? returnDate = null;
                    Librarian? libReturn = null;
                    
                    bool createPaidFine = paidFinesCount < 135;
                    bool createPendingFine = !createPaidFine && pendingFinesCount < 15;
                    
                    if (createPaidFine)
                    {
                        // Returned late
                        var diffDays = random.Next(1, 20);
                        returnDate = dueDate.AddDays(diffDays);
                        if (returnDate > DateTime.Today) returnDate = DateTime.Today;
                        
                        libReturn = librarians[random.Next(librarians.Count)];
                        book.AvailableCopies++;
                        book.IsAvailable = true;
                        
                        var record = new BorrowRecord
                        {
                            StudentId = student.Id,
                            BookId = book.Id,
                            BorrowDate = borrowDate,
                            DueDate = dueDate,
                            IssuedByLibrarianId = libIssue.Id,
                            ReturnDate = returnDate,
                            ReturnedByLibrarianId = libReturn.Id
                        };
                        borrowRecords.Add(record);
                        
                        fines.Add(new Fine
                        {
                            BorrowRecord = record,
                            StudentId = student.Id,
                            Amount = diffDays * 10m,
                            GeneratedDate = returnDate.Value,
                            Status = "Paid",
                            PaidDate = returnDate.Value,
                            Reason = "Late Return"
                        });
                        paidFinesCount++;
                    }
                    else if (createPendingFine)
                    {
                        // Overdue, not returned
                        borrowDate = DateTime.Today.AddDays(-random.Next(20, 100)); // definitely overdue
                        dueDate = borrowDate.AddDays(15);
                        
                        var record = new BorrowRecord
                        {
                            StudentId = student.Id,
                            BookId = book.Id,
                            BorrowDate = borrowDate,
                            DueDate = dueDate,
                            IssuedByLibrarianId = libIssue.Id,
                            ReturnDate = null
                        };
                        borrowRecords.Add(record);
                        
                        var diffDays = (int)(DateTime.Today - dueDate).TotalDays;
                        fines.Add(new Fine
                        {
                            BorrowRecord = record,
                            StudentId = student.Id,
                            Amount = diffDays * 10m,
                            GeneratedDate = DateTime.Today.AddDays(-1),
                            Status = "Pending",
                            Reason = "Overdue Book"
                        });
                        pendingFinesCount++;
                    }
                    else
                    {
                        // Normal record (returned on time, or active and not overdue)
                        if (random.Next(100) < 85) // 85% returned on time
                        {
                            var returnDays = random.Next(1, 15);
                            returnDate = borrowDate.AddDays(returnDays);
                            if (returnDate > DateTime.Today) returnDate = DateTime.Today;
                            
                            libReturn = librarians[random.Next(librarians.Count)];
                            book.AvailableCopies++;
                            book.IsAvailable = true;
                        }
                        else
                        {
                            // Active, not overdue
                            borrowDate = DateTime.Today.AddDays(-random.Next(0, 14));
                            dueDate = borrowDate.AddDays(15);
                        }
                        
                        var record = new BorrowRecord
                        {
                            StudentId = student.Id,
                            BookId = book.Id,
                            BorrowDate = borrowDate,
                            DueDate = dueDate,
                            IssuedByLibrarianId = libIssue.Id,
                            ReturnDate = returnDate,
                            ReturnedByLibrarianId = libReturn?.Id
                        };
                        borrowRecords.Add(record);
                    }
                }
            }
            
            context.BorrowRecords.AddRange(borrowRecords);
            context.Fines.AddRange(fines);
            
            context.SaveChanges();
        }
    }
}
