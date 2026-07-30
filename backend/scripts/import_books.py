import os
import sys
import logging
import time
from datetime import datetime
import pandas as pd
from dotenv import load_dotenv
from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker

# Ensure backend directory is in the path so we can import models
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))
from models import Book

# Setup logging
log_dir = os.path.join(os.path.dirname(os.path.dirname(__file__)), 'logs')
os.makedirs(log_dir, exist_ok=True)
log_file = os.path.join(log_dir, 'book_import.log')

logging.basicConfig(
    filename=log_file,
    level=logging.ERROR,
    format='%(asctime)s - %(levelname)s - %(message)s'
)
console_handler = logging.StreamHandler()
console_handler.setLevel(logging.INFO)
logger = logging.getLogger()
logger.addHandler(console_handler)

def parse_adonet_connection_string(conn_str):
    """Parses an ADO.NET connection string into a SQLAlchemy URI."""
    parts = conn_str.split(';')
    params = {}
    for part in parts:
        if '=' in part:
            key, val = part.split('=', 1)
            params[key.strip().lower()] = val.strip()
    
    if not all(k in params for k in ['host', 'port', 'database', 'username', 'password']):
        raise ValueError("Invalid ADO.NET connection string format for parsing.")

    return f"postgresql+psycopg2://{params['username']}:{params['password']}@{params['host']}:{params['port']}/{params['database']}"

def parse_date(date_str):
    """Parses mixed date strings into a valid datetime.date object."""
    if pd.isna(date_str) or not str(date_str).strip():
        return None
    
    date_str = str(date_str).strip()
    
    try:
        # Try YYYY-MM-DD
        if len(date_str) == 10:
            return datetime.strptime(date_str, "%Y-%m-%d").date()
        # Try YYYY-MM
        elif len(date_str) == 7:
            return datetime.strptime(date_str, "%Y-%m").date()
        # Try YYYY
        elif len(date_str) == 4:
            return datetime.strptime(date_str, "%Y").date()
        else:
            # Fallback for unexpected formats
            dt = pd.to_datetime(date_str, errors='coerce')
            if pd.isna(dt):
                return None
            return dt.date()
    except Exception:
        return None

def main():
    start_time = time.time()
    
    # 1. Load ENV
    env_path = os.path.abspath(os.path.join(os.path.dirname(__file__), '..', '..', 'LibraryManagement.MVC', '.env'))
    load_dotenv(env_path)
    
    conn_str = os.getenv('DATABASE_URL')
    if not conn_str:
        logger.error("DATABASE_URL not found in .env file.")
        sys.exit(1)
        
    try:
        if "Host=" in conn_str:
            db_url = parse_adonet_connection_string(conn_str)
        else:
            db_url = conn_str.replace("postgresql+asyncpg://", "postgresql+psycopg2://")
    except Exception as e:
        logger.error(f"Failed to parse connection string: {e}")
        sys.exit(1)
        
    # 2. Setup Database
    engine = create_engine(db_url)
    SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)
    session = SessionLocal()
    
    # 3. Cache existing records to skip duplicates efficiently
    logger.info("Caching existing records from database to prevent duplicates...")
    existing_books = session.query(Book.ISBN, Book.Title, Book.Author).all()
    
    existing_isbns = set(b.ISBN for b in existing_books if b.ISBN)
    existing_title_authors = set((b.Title.lower(), b.Author.lower()) for b in existing_books if b.Title and b.Author)
    
    # 4. Read CSV
    csv_path = os.path.abspath(os.path.join(os.path.dirname(__file__), '..', '..', 'LibraryManagement.MVC', 'Dataset', 'google_books_dataset.csv'))
    if not os.path.exists(csv_path):
        logger.error(f"Dataset not found at {csv_path}")
        sys.exit(1)
        
    logger.info("Reading CSV dataset...")
    df = pd.read_csv(csv_path)
    total_csv_records = len(df)
    
    imported_count = 0
    skipped_missing_title = 0
    skipped_duplicate = 0
    skipped_invalid = 0
    
    batch_size = 500
    current_batch = []
    
    # Used to track duplicates within the CSV itself during processing
    seen_isbns = set()
    seen_title_authors = set()
    
    logger.info(f"Starting import of {total_csv_records} records...")
    
    for index, row in df.iterrows():
        # Clean data (Trim strings, handle NaNs)
        cleaned_row = {}
        for col in df.columns:
            val = row[col]
            if pd.isna(val):
                cleaned_row[col] = None
            elif isinstance(val, str):
                cleaned_row[col] = val.strip()
            else:
                cleaned_row[col] = val
                
        # 3. Ignore rows where Title is missing
        title = cleaned_row.get('title')
        if not title:
            skipped_missing_title += 1
            continue
            
        author = cleaned_row.get('authors')
        if not author:
            author = "Unknown Author"
        
        isbn = cleaned_row.get('isbn_13')
        
        # Deduplication Logic
        is_duplicate = False
        
        # Check ISBN
        if isbn:
            if isbn in existing_isbns or isbn in seen_isbns:
                is_duplicate = True
        
        # Check Title + Author
        title_author_key = (title.lower(), author.lower())
        if title_author_key in existing_title_authors or title_author_key in seen_title_authors:
            is_duplicate = True
            
        if is_duplicate:
            skipped_duplicate += 1
            continue
            
        # Parse Dates
        published_date = parse_date(cleaned_row.get('published_date'))
        
        category = cleaned_row.get('categories')
        if not category:
            category = 'Uncategorized'
            
        # Create Book model
        try:
            book = Book(
                Title=title[:200] if title else None,
                Author=author[:150] if author else None,
                ISBN = (
                    str(isbn).strip()
                    if isbn and str(isbn).strip()
                    else None
                ),
                Category=category[:100],
                TotalCopies=5,
                AvailableCopies=5,
                IsAvailable=True,
                Publisher=cleaned_row.get('publisher'),
                PublishedDate=published_date,
                Description=cleaned_row.get('description'),
                PageCount=int(cleaned_row['page_count']) if cleaned_row.get('page_count') is not None else None,
                Language=cleaned_row.get('language'),
                AverageRating=float(cleaned_row['average_rating']) if cleaned_row.get('average_rating') is not None else None,
                RatingsCount=int(cleaned_row['ratings_count']) if cleaned_row.get('ratings_count') is not None else None,
                Thumbnail=cleaned_row.get('thumbnail'),
                GoogleBookId=cleaned_row.get('book_id')
            )
            current_batch.append(book)
            
            # Add to local seen tracking
            if isbn:
                seen_isbns.add(isbn)
            seen_title_authors.add(title_author_key)
            
        except Exception as e:
            logger.error(f"Error parsing row {index}: {e}")
            skipped_invalid += 1
            continue
            
        # Insert batch
        if len(current_batch) >= batch_size:
            try:
                session.add_all(current_batch)
                session.commit()
                imported_count += len(current_batch)
                print(f"Imported {imported_count} / {total_csv_records}")
            except Exception as e:
                session.rollback()
                logger.error(f"Batch insert failed, rolling back current batch of 500. Error: {e}")
                skipped_invalid += len(current_batch)
            finally:
                current_batch = []
                
    # Insert remaining records
    if current_batch:
        try:
            session.add_all(current_batch)
            session.commit()
            imported_count += len(current_batch)
            print(f"Imported {imported_count} / {total_csv_records}")
        except Exception as e:
            session.rollback()
            logger.error(f"Final batch insert failed, rolling back. Error: {e}")
            skipped_invalid += len(current_batch)
            
    session.close()
    
    elapsed_time = time.time() - start_time
    minutes, seconds = divmod(elapsed_time, 60)
    
    print("\n" + "="*30)
    print("IMPORT SUMMARY")
    print("="*30)
    print(f"Total CSV Records:       {total_csv_records}")
    print(f"Imported:                {imported_count}")
    print(f"Skipped Missing Title:   {skipped_missing_title}")
    print(f"Skipped Duplicate:       {skipped_duplicate}")
    print(f"Skipped Invalid Data:    {skipped_invalid}")
    print(f"Total Time:              {int(minutes)}m {int(seconds)}s")
    print("="*30)

if __name__ == "__main__":
    main()
