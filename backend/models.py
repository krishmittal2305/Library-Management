from sqlalchemy import Column, Integer, String, Boolean, Date, Text, Numeric
from sqlalchemy.orm import declarative_base

Base = declarative_base()

class Book(Base):
    __tablename__ = "Books"

    Id = Column(Integer, primary_key=True, autoincrement=True)
    Title = Column(String(200), nullable=False)
    Author = Column(String(150), nullable=False)
    ISBN = Column(String, nullable=False)
    Category = Column(String, nullable=False)
    TotalCopies = Column(Integer, nullable=False, default=5)
    AvailableCopies = Column(Integer, nullable=False, default=5)
    IsAvailable = Column(Boolean, nullable=False, default=True)
    
    Publisher = Column(String, nullable=True)
    PublishedDate = Column(Date, nullable=True)
    Description = Column(Text, nullable=True)
    PageCount = Column(Integer, nullable=True)
    Language = Column(String, nullable=True)
    AverageRating = Column(Numeric(precision=3, scale=2), nullable=True)
    RatingsCount = Column(Integer, nullable=True)
    Thumbnail = Column(String, nullable=True)
    GoogleBookId = Column(String, nullable=True)
