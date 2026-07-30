/**
 * books.js
 * Seed data for the Library Management System.
 * Acts as the "database" for the frontend-only application.
 * Data is persisted in localStorage so changes survive page reloads.
 */

// ─── Seed Data ────────────────────────────────────────────────────────────────
const SEED_BOOKS = [
  {
    id: 1,
    title: "The Great Gatsby",
    author: "F. Scott Fitzgerald",
    isbn: "978-0-7432-7356-5",
    publishedDate: "1925-04-10",
    available: true,
    borrower: null
  },
  {
    id: 2,
    title: "To Kill a Mockingbird",
    author: "Harper Lee",
    isbn: "978-0-06-112008-4",
    publishedDate: "1960-07-11",
    available: true,
    borrower: null
  },
  {
    id: 3,
    title: "1984",
    author: "George Orwell",
    isbn: "978-0-452-28423-4",
    publishedDate: "1949-06-08",
    available: false,
    borrower: {
      studentId: 1, // Foreign key to Alice
      borrowDate: "2026-07-10"
    }
  },
  {
    id: 4,
    title: "Pride and Prejudice",
    author: "Jane Austen",
    isbn: "978-0-14-143951-8",
    publishedDate: "1813-01-28",
    available: true,
    borrower: null
  },
  {
    id: 5,
    title: "The Catcher in the Rye",
    author: "J.D. Salinger",
    isbn: "978-0-316-76948-0",
    publishedDate: "1951-07-16",
    available: true,
    borrower: null
  },
  {
    id: 6,
    title: "Brave New World",
    author: "Aldous Huxley",
    isbn: "978-0-06-085052-4",
    publishedDate: "1932-08-30",
    available: false,
    borrower: {
      studentId: 2, // Foreign key to Bob
      borrowDate: "2026-07-15"
    }
  },
  {
    id: 7,
    title: "The Hobbit",
    author: "J.R.R. Tolkien",
    isbn: "978-0-547-92822-7",
    publishedDate: "1937-09-21",
    available: true,
    borrower: null
  }
];

// ─── Storage Keys ─────────────────────────────────────────────────────────────
const STORAGE_KEY = "lms_books";
const ID_COUNTER_KEY = "lms_next_id";

/**
 * Initialise the book store.
 * If localStorage has no data yet, seed it with SEED_BOOKS.
 */
function initBooks() {
  if (!localStorage.getItem(STORAGE_KEY)) {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(SEED_BOOKS));
    localStorage.setItem(ID_COUNTER_KEY, "8"); // Next available ID
  }
}

/** Return all books as a JavaScript array. */
function getAllBooks() {
  return JSON.parse(localStorage.getItem(STORAGE_KEY)) || [];
}

/** Return a single book by numeric id, or null if not found. */
function getBookById(id) {
  return getAllBooks().find(b => b.id === Number(id)) || null;
}

/** Persist the full books array back to localStorage. */
function saveBooks(books) {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(books));
}

/** Add a new book. Auto-assigns an incrementing id. Returns the new book. */
function addBook(bookData) {
  const books = getAllBooks();
  const nextId = parseInt(localStorage.getItem(ID_COUNTER_KEY) || "1", 10);
  const newBook = {
    id: nextId,
    title: bookData.title.trim(),
    author: bookData.author.trim(),
    isbn: bookData.isbn.trim(),
    publishedDate: bookData.publishedDate,
    available: true,
    borrower: null
  };
  books.push(newBook);
  saveBooks(books);
  localStorage.setItem(ID_COUNTER_KEY, String(nextId + 1));
  return newBook;
}

/** Update an existing book by id. Returns updated book or null. */
function updateBook(id, updates) {
  const books = getAllBooks();
  const idx = books.findIndex(b => b.id === Number(id));
  if (idx === -1) return null;
  books[idx] = { ...books[idx], ...updates };
  saveBooks(books);
  return books[idx];
}

/** Delete a book by id. Returns true on success. */
function deleteBook(id) {
  let books = getAllBooks();
  const before = books.length;
  books = books.filter(b => b.id !== Number(id));
  if (books.length === before) return false;
  saveBooks(books);
  return true;
}

/** Borrow a book – sets available:false and attaches borrower info (studentId). */
function borrowBook(id, studentId) {
  const today = new Date().toISOString().split("T")[0];
  const historyId = Date.now().toString() + Math.floor(Math.random()*1000); // simulate unique BorrowRecordId
  
  // Log history
  const history = JSON.parse(localStorage.getItem("lms_history")) || [];
  history.push({ historyId, id, studentId: Number(studentId), action: "borrow", date: today });
  localStorage.setItem("lms_history", JSON.stringify(history));

  return updateBook(id, {
    available: false,
    borrower: {
      historyId,
      studentId: Number(studentId),
      borrowDate: today
    }
  });
}

/** Return a book – sets available:true and clears borrower info. */
function returnBook(id) {
  const today = new Date().toISOString().split("T")[0];
  
  const books = getAllBooks();
  const book = books.find(b => b.id === Number(id));
  const studentId = (book && book.borrower) ? book.borrower.studentId : null;
  const historyId = (book && book.borrower) ? book.borrower.historyId : null;

  // Log history
  const history = JSON.parse(localStorage.getItem("lms_history")) || [];
  if (historyId) {
    history.push({ historyId, id, studentId, action: "return", date: today });
  } else {
    // Fallback for older seed data
    history.push({ id, studentId, action: "return", date: today });
  }
  localStorage.setItem("lms_history", JSON.stringify(history));

  return updateBook(id, { available: true, borrower: null });
}

// Initialise on script load
initBooks();
