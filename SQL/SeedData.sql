USE [LibraryManagementSystem]
GO
/****** Script Date: 8/23/2026 7:42:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Insert Sample Data
-- =============================================

-- 1. Insert Publishers
SET IDENTITY_INSERT [Publishers] ON;
INSERT INTO [Publishers] ([PublisherId], [Name]) VALUES 
(1, 'Penguin Random House'),
(2, 'HarperCollins'),
(3, 'O''Reilly Media');
SET IDENTITY_INSERT [Publishers] OFF;
GO

-- 2. Insert Authors
SET IDENTITY_INSERT [Authors] ON;
INSERT INTO [Authors] ([AuthorId], [FirstName], [LastName], [Bio]) VALUES 
(1, 'George', 'Orwell', 'English novelist and essayist, journalist and critic.'),
(2, 'J.K.', 'Rowling', 'British author, best known for the Harry Potter series.'),
(3, 'Robert C.', 'Martin', 'American software engineer and author, also known as Uncle Bob.'),
(4, 'Martin', 'Fowler', 'British software developer and author.'),
(5, 'Stephen', 'King', 'American author of horror, supernatural fiction, suspense, and fantasy novels.');
SET IDENTITY_INSERT [Authors] OFF;
GO

-- 3. Insert Categories (Hierarchical)
SET IDENTITY_INSERT [Categories] ON;
INSERT INTO [Categories] ([CategoryId], [Name], [ParentCategoryId]) VALUES 
(1, 'Fiction', NULL),
(2, 'Science Fiction', 1),
(3, 'Programming', NULL),
(4, 'Database', 3),
(5, 'History', NULL);
SET IDENTITY_INSERT [Categories] OFF;
GO

-- 4. Insert Books
SET IDENTITY_INSERT [Books] ON;
INSERT INTO [Books] ([BookId], [Title], [ISBN], [PublisherId], [Edition], [Language], [PublicationYear], [PageCount], [IsDeleted], [CreatedAt], [UpdatedAt]) VALUES 
(1, 'Clean Code', '9780132350884', 3, '1st Edition', 'English', 2008, 464, 0, GETUTCDATE(), GETUTCDATE()),
(2, '1984', '9780451524935', 1, 'Reprint Edition', 'English', 1949, 328, 0, GETUTCDATE(), GETUTCDATE());
SET IDENTITY_INSERT [Books] OFF;
GO

-- 5. Link Books and Authors (Many-to-Many)
INSERT INTO [BookAuthors] ([BookId], [AuthorId]) VALUES 
(1, 3), -- Clean Code by Robert C. Martin
(2, 1); -- 1984 by George Orwell
GO

-- 6. Link Books and Categories (Many-to-Many)
INSERT INTO [BookCategories] ([BookId], [CategoryId]) VALUES 
(1, 3), -- Clean Code -> Programming
(2, 1), -- 1984 -> Fiction
(2, 2); -- 1984 -> Science Fiction
GO

-- 7. Insert Book Copies
SET IDENTITY_INSERT [BookCopies] ON;
INSERT INTO [BookCopies] ([CopyId], [BookId], [Barcode], [Status], [CreatedAt]) VALUES 
(1, 1, 'BC-001-CC', 'Available', GETUTCDATE()),
(2, 1, 'BC-002-CC', 'Available', GETUTCDATE()),
(3, 2, 'BC-001-1984', 'Available', GETUTCDATE());
SET IDENTITY_INSERT [BookCopies] OFF;
GO

-- 8. Insert Members
SET IDENTITY_INSERT [Members] ON;
INSERT INTO [Members] ([MemberId], [MembershipNumber], [FirstName], [LastName], [Email], [Phone], [Address], [JoinDate], [ExpiryDate], [Status], [CreatedAt], [UpdatedAt]) VALUES 
(1, 'MEM-001', 'John', 'Doe', 'john.doe@example.com', '555-0100', '123 Main St', CAST(GETUTCDATE() AS DATE), CAST(DATEADD(year, 1, GETUTCDATE()) AS DATE), 'Active', GETUTCDATE(), GETUTCDATE()),
(2, 'MEM-002', 'Jane', 'Smith', 'jane.smith@example.com', '555-0200', '456 Oak Ave', CAST(GETUTCDATE() AS DATE), CAST(DATEADD(year, 1, GETUTCDATE()) AS DATE), 'Active', GETUTCDATE(), GETUTCDATE());
SET IDENTITY_INSERT [Members] OFF;
GO
