INSERT INTO
    Books (
        BookName,
        Category,
        Price,
        Description,
        ImageUrl
    )
VALUES (
        'To Kill a Mockingbird',
        'Fiction',
        12.99,
        'A classic novel exploring themes of justice and morality in the American South.',
        'https://res.cloudinary.com/eves-oasis/image/upload/v1735242006/mainbg_p8bltp.jpg'
    ),
    (
        '1984',
        'Dystopian',
        9.99,
        'George Orwell''s chilling vision of a totalitarian future.',
        'https://res.cloudinary.com/eves-oasis/image/upload/v1735242006/mainbg_p8bltp.jpg'
    ),
    (
        'The Great Gatsby',
        'Fiction',
        10.49,
        'A tale of wealth, love, and the American Dream in the Roaring Twenties.',
        'https://res.cloudinary.com/eves-oasis/image/upload/v1735242006/mainbg_p8bltp.jpg'
    ),
    (
        'Clean Code',
        'Technology',
        39.99,
        'A handbook on writing clean, readable, and maintainable software code.',
        'https://res.cloudinary.com/eves-oasis/image/upload/v1735242006/mainbg_p8bltp.jpg'
    ),
    (
        'Sapiens: A Brief History of Humankind',
        'Non-Fiction',
        14.99,
        'An exploration of the history and impact of Homo sapiens.',
        'https://res.cloudinary.com/eves-oasis/image/upload/v1735242006/mainbg_p8bltp.jpg'
    ),
    (
        'Dune',
        'Science Fiction',
        11.99,
        'A science fiction epic set on the desert planet Arrakis.',
        'https://res.cloudinary.com/eves-oasis/image/upload/v1735242006/mainbg_p8bltp.jpg'
    ),
    (
        'The Catcher in the Rye',
        'Fiction',
        8.99,
        'A coming-of-age story about teenage angst and rebellion.',
        'https://res.cloudinary.com/eves-oasis/image/upload/v1735242006/mainbg_p8bltp.jpg'
    ),
    (
        'Python Crash Course',
        'Technology',
        29.99,
        'A comprehensive guide to Python programming for beginners and intermediates.',
        'https://res.cloudinary.com/eves-oasis/image/upload/v1735242006/mainbg_p8bltp.jpg'
    ),
    (
        'Pride and Prejudice',
        'Romance',
        7.99,
        'Jane Austen''s timeless novel of love and social class.',
        'https://res.cloudinary.com/eves-oasis/image/upload/v1735242006/mainbg_p8bltp.jpg'
    ),
    (
        'The Hobbit',
        'Fantasy',
        13.49,
        'An adventurous tale of Bilbo Baggins in J.R.R. Tolkien''s Middle-earth.',
        'https://res.cloudinary.com/eves-oasis/image/upload/v1735242006/mainbg_p8bltp.jpg'
    );

-- drop table if exists books;

-- Update existing books with authors based on their IDs
UPDATE Books
SET
    Author = CASE BookId
        WHEN 11 THEN 'Harper Lee'
        WHEN 13 THEN 'George Orwell'
        WHEN 14 THEN 'F. Scott Fitzgerald'
        WHEN 16 THEN 'J.K. Rowling'
        WHEN 17 THEN 'Jane Austen'
        WHEN 18 THEN 'Stephen King'
        WHEN 19 THEN 'Agatha Christie'
        WHEN 20 THEN 'Dan Brown'
        WHEN 22 THEN 'J.R.R. Tolkien'
        WHEN 25 THEN 'Ernest Hemingway'
        WHEN 26 THEN 'Mark Twain'
        WHEN 27 THEN 'Charles Dickens'
        WHEN 28 THEN 'William Shakespeare'
        ELSE 'Unknown Author'
    END,
    CreatedAt = CASE
        WHEN CreatedAt IS NULL THEN NOW()
        ELSE CreatedAt
    END,
    UpdatedAt = NOW()
WHERE
    BookId IN (
        11,
        13,
        14,
        16,
        17,
        18,
        19,
        20,
        22,
        25,
        26,
        27,
        28
    );