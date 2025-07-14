-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Jul 14, 2025 at 01:27 AM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `bookstore`
--

-- --------------------------------------------------------

--
-- Table structure for table `books`
--

CREATE TABLE `books` (
  `BookId` int(11) NOT NULL,
  `BookName` varchar(255) NOT NULL,
  `Category` longtext NOT NULL,
  `Price` decimal(18,2) NOT NULL,
  `Description` longtext NOT NULL,
  `ImageUrl` longtext NOT NULL,
  `Author` longtext NOT NULL,
  `CreatedAt` datetime(6) NOT NULL DEFAULT current_timestamp(6),
  `UpdatedAt` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `books`
--

INSERT INTO `books` (`BookId`, `BookName`, `Category`, `Price`, `Description`, `ImageUrl`, `Author`, `CreatedAt`, `UpdatedAt`) VALUES
(11, 'To Kill a Mockingbird', 'Fiction', 12.99, 'A classic novel exploring themes of justice and morality in the American South.', 'https://res.cloudinary.com/eves-oasis/image/upload/v1735242006/mainbg_p8bltp.jpg', 'Harper Lee', '2025-07-11 04:23:26.774111', '2025-07-11 04:26:20.000000'),
(13, 'The Great Gatsby', 'Fiction', 10.49, 'A tale of wealth, love, and the American Dream in the Roaring Twenties.', 'https://res.cloudinary.com/eves-oasis/image/upload/c_crop,ar_1:1/v1732258031/samples/breakfast.jpg', 'George Orwell', '2025-07-11 04:23:26.774111', '2025-07-11 04:26:20.000000'),
(14, 'Clean Code', 'Technology', 39.99, 'A handbook on writing clean, readable, and maintainable software code.', 'https://images.pexels.com/photos/904620/pexels-photo-904620.jpeg', 'F. Scott Fitzgerald', '2025-07-11 04:23:26.774111', '2025-07-11 04:26:20.000000'),
(16, 'Dune', 'Science Fiction', 11.99, 'A science fiction epic set on the desert planet Arrakis.', 'https://images.pexels.com/photos/3747279/pexels-photo-3747279.jpeg', 'J.K. Rowling', '2025-07-11 04:23:26.774111', '2025-07-11 04:26:20.000000'),
(17, 'The Catcher in the Rye', 'Fiction', 8.99, 'A coming-of-age story about teenage angst and rebellion.', 'https://images.pexels.com/photos/1765033/pexels-photo-1765033.jpeg', 'Jane Austen', '2025-07-11 04:23:26.774111', '2025-07-11 04:26:20.000000'),
(18, 'Python Crash Course', 'Technology', 29.99, 'A comprehensive guide to Python programming for beginners and intermediates.', 'https://images.pexels.com/photos/3990897/pexels-photo-3990897.jpeg', 'Stephen King', '2025-07-11 04:23:26.774111', '2025-07-11 04:26:20.000000'),
(20, 'The Hobbit', 'Fantasy', 13.49, 'An adventurous tale of Bilbo Baggins in J.R.R. Tolkien\'s Middle-earth.', 'https://images.pexels.com/photos/2228582/pexels-photo-2228582.jpeg', 'Dan Brown', '2025-07-11 04:23:26.774111', '2025-07-11 04:26:20.000000'),
(22, 'To Kill A Mocking Bird', 'Fiction', 45.00, 'How To Kill A Mocking Bird', 'https://images.pexels.com/photos/4547588/pexels-photo-4547588.jpeg', 'J.R.R. Tolkien', '2025-07-11 04:23:26.774111', '2025-07-11 04:26:20.000000'),
(25, 'The Dean Of Eddy', 'Self-Help', 55.00, 'what are we to do', 'uploads/books/b6cd7d83-9679-4d23-9e23-c4d5ff781880.jpg', 'Ernest Hemingway', '2025-07-11 04:23:26.774111', '2025-07-11 04:26:20.000000'),
(26, 'The Last Supper ', 'History', 95.00, 'When it is time to eat there will be no disturbances', 'http://localhost:8080/uploads/books/2152f8cb-11eb-4564-9f86-505ef91ab123.jpg', 'Mark Twain', '2025-07-11 04:23:26.774111', '2025-07-11 04:26:20.000000'),
(29, 'Noah Goes To china', 'Science Fiction', 50.00, 'When a young man decides to leave home, what will be his faith in a new environment of an unknown world?', 'http://localhost:8080/uploads/books/47e125d0-56cd-418f-852b-28b41aff03c3.jpg', 'Sharp Pears', '2025-07-11 05:10:10.972847', '2025-07-13 01:39:34.782044'),
(30, 'What Did ?', 'Fiction', 45.00, 'How To Kill A Mocking Bird', 'https://res.cloudinary.com/eves-oasis/image/upload/v1735242006/mainbg_p8bltp.jpg', 'Heny Wasp', '2025-07-11 05:10:42.968609', NULL),
(32, 'Bacon And Avocado', 'Science Fiction', 53.00, 'What are we waiting for ', 'http://localhost:8080/uploads/books/f367cce8-9982-4ac3-a1e6-9a959466e7b8.jpg', 'Post Malone', '2025-07-11 14:54:44.269475', NULL),
(34, 'The Life Of Pablo Escobar', 'Biography', 70.00, 'Witness the sudden rise and fall of the notorious Mexican drug lord', 'http://localhost:8080/uploads/books/6dbe4bf2-2548-4509-b217-28e2385f95cd.jpg', 'Luis Capaldi', '2025-07-12 22:10:28.910251', '2025-07-13 09:47:51.372922'),
(35, 'Queen', 'Mystery', 77.00, 'All the adventures and life of the Queen band members before and after fame', 'http://localhost:8080/uploads/books/54733cca-4fa4-4467-aca8-77dd22029139.jpg', 'Brian May', '2025-07-12 22:34:08.859595', '2025-07-13 09:50:48.295295');

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `Id` int(11) NOT NULL,
  `Name` longtext NOT NULL,
  `Email` varchar(255) NOT NULL,
  `Password` longtext NOT NULL,
  `PhoneNumber` longtext DEFAULT NULL,
  `Address` longtext DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`Id`, `Name`, `Email`, `Password`, `PhoneNumber`, `Address`, `CreatedAt`, `UpdatedAt`) VALUES
(1, 'Bright Amoah', 'brghtmalone@gmail.com', '$2a$11$acYCPmyqNf0Oj9gbRk.nfOlrE0PJaCYVc9lNYX1cMuUPIVD8etoIu', NULL, NULL, '2025-07-08 12:33:05.421321', '2025-07-08 12:33:05.421321'),
(6, 'Post Malone', 'lottemarie77@gmail.com', '$2a$11$36yPlW76DCLmjdfIKb/3AeB4YTgRblFUfuAIjZPiv5JxDdxMV9foK', NULL, NULL, '2025-07-08 19:07:41.896427', '2025-07-08 19:07:41.896428'),
(7, 'Bright Amoah', 'brightphenomenalamoah@gmail.com', '$2a$11$TWZR6S5UkvUNHzaiJkgg.OJ4dovJt94u2J5sEgyJu8JT9TLioUV1C', NULL, NULL, '2025-07-09 10:44:31.879587', '2025-07-09 10:44:31.879588'),
(8, 'Marie Lotte', 'lottemarie77+1@gmail.com', '$2a$11$IyfF1n4WKFxlz1R4NtTmEuw4Ai9SKEbC8puT1B6QyUplwG9hIVQtm', NULL, NULL, '2025-07-09 11:11:10.726598', '2025-07-09 11:11:10.726598'),
(9, 'Post Malone', 'lottemarie77+5@gmail.com', '$2a$11$OKhMzUJXuNfi6tbjxrNvUuzbL0XWoE7/pviApbaBqo4aNPHaP02nq', NULL, NULL, '2025-07-10 03:01:50.879975', '2025-07-10 03:01:50.879975'),
(11, 'Bright Amoah', 'bkamoah02+2@gmail.com', '$2a$11$AGKZB9lSZzJa9jvY/pX/4OJkIODUeJGX7zsEbbSxZ6EF1paRaCgJa', NULL, NULL, '2025-07-13 02:21:37.114491', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `__efmigrationshistory`
--

CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `__efmigrationshistory`
--

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES
('20250708122159_InitialCreate', '8.0.5'),
('20250710011537_AddBooksTable', '8.0.5'),
('20250711042249_UpdateBooksTableWithAuthorAndTimestamps', '8.0.5');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `books`
--
ALTER TABLE `books`
  ADD PRIMARY KEY (`BookId`),
  ADD KEY `IX_Books_BookName` (`BookName`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `IX_Users_Email` (`Email`);

--
-- Indexes for table `__efmigrationshistory`
--
ALTER TABLE `__efmigrationshistory`
  ADD PRIMARY KEY (`MigrationId`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `books`
--
ALTER TABLE `books`
  MODIFY `BookId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=36;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
