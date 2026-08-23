CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [IsActive] bit NOT NULL,
    [LastLoginAt] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [Authors] (
    [AuthorId] int NOT NULL IDENTITY,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [Bio] nvarchar(1000) NULL,
    CONSTRAINT [PK_Authors] PRIMARY KEY ([AuthorId])
);
GO


CREATE TABLE [Categories] (
    [CategoryId] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NULL,
    [ParentCategoryId] int NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([CategoryId]),
    CONSTRAINT [FK_Categories_Categories_ParentCategoryId] FOREIGN KEY ([ParentCategoryId]) REFERENCES [Categories] ([CategoryId]) ON DELETE NO ACTION
);
GO


CREATE TABLE [Members] (
    [MemberId] int NOT NULL IDENTITY,
    [MembershipNumber] nvarchar(20) NOT NULL,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [Email] nvarchar(200) NOT NULL,
    [Phone] nvarchar(20) NULL,
    [Address] nvarchar(300) NULL,
    [JoinDate] date NOT NULL,
    [ExpiryDate] date NULL,
    [Status] nvarchar(20) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Members] PRIMARY KEY ([MemberId])
);
GO


CREATE TABLE [Publishers] (
    [PublisherId] int NOT NULL IDENTITY,
    [Name] nvarchar(200) NOT NULL,
    [Email] nvarchar(200) NULL,
    [Phone] nvarchar(20) NULL,
    CONSTRAINT [PK_Publishers] PRIMARY KEY ([PublisherId])
);
GO


CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [UserActivityLogs] (
    [LogId] bigint NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [Action] nvarchar(50) NOT NULL,
    [Entity] nvarchar(100) NOT NULL,
    [EntityId] int NULL,
    [Details] nvarchar(500) NULL,
    [Timestamp] datetime2 NOT NULL,
    CONSTRAINT [PK_UserActivityLogs] PRIMARY KEY ([LogId]),
    CONSTRAINT [FK_UserActivityLogs_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
);
GO


CREATE TABLE [Books] (
    [BookId] int NOT NULL IDENTITY,
    [ISBN] nvarchar(20) NOT NULL,
    [Title] nvarchar(500) NOT NULL,
    [Edition] nvarchar(50) NULL,
    [Language] nvarchar(50) NULL,
    [PublicationYear] smallint NULL,
    [Summary] nvarchar(2000) NULL,
    [CoverImageUrl] nvarchar(500) NULL,
    [PageCount] int NULL,
    [PublisherId] int NOT NULL,
    [IsDeleted] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Books] PRIMARY KEY ([BookId]),
    CONSTRAINT [FK_Books_Publishers_PublisherId] FOREIGN KEY ([PublisherId]) REFERENCES [Publishers] ([PublisherId]) ON DELETE CASCADE
);
GO


CREATE TABLE [BookAuthors] (
    [AuthorId] int NOT NULL,
    [BookId] int NOT NULL,
    CONSTRAINT [PK_BookAuthors] PRIMARY KEY ([AuthorId], [BookId]),
    CONSTRAINT [FK_BookAuthors_Authors_AuthorId] FOREIGN KEY ([AuthorId]) REFERENCES [Authors] ([AuthorId]) ON DELETE CASCADE,
    CONSTRAINT [FK_BookAuthors_Books_BookId] FOREIGN KEY ([BookId]) REFERENCES [Books] ([BookId]) ON DELETE CASCADE
);
GO


CREATE TABLE [BookCategories] (
    [CategoryId] int NOT NULL,
    [BookId] int NOT NULL,
    CONSTRAINT [PK_BookCategories] PRIMARY KEY ([CategoryId], [BookId]),
    CONSTRAINT [FK_BookCategories_Books_BookId] FOREIGN KEY ([BookId]) REFERENCES [Books] ([BookId]) ON DELETE CASCADE,
    CONSTRAINT [FK_BookCategories_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([CategoryId]) ON DELETE CASCADE
);
GO


CREATE TABLE [BookCopies] (
    [CopyId] int NOT NULL IDENTITY,
    [BookId] int NOT NULL,
    [Barcode] nvarchar(50) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [ShelfLocation] nvarchar(100) NULL,
    [Notes] nvarchar(500) NULL,
    [PurchasePrice] decimal(18,2) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_BookCopies] PRIMARY KEY ([CopyId]),
    CONSTRAINT [FK_BookCopies_Books_BookId] FOREIGN KEY ([BookId]) REFERENCES [Books] ([BookId]) ON DELETE CASCADE
);
GO


CREATE TABLE [BorrowingTransactions] (
    [TransactionId] bigint NOT NULL IDENTITY,
    [CopyId] int NOT NULL,
    [MemberId] int NOT NULL,
    [IssuedByUserId] nvarchar(450) NOT NULL,
    [ReturnedToUserId] nvarchar(450) NULL,
    [BorrowedAt] datetime2 NOT NULL,
    [DueDate] datetime2 NOT NULL,
    [ReturnDate] datetime2 NULL,
    [Status] nvarchar(20) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_BorrowingTransactions] PRIMARY KEY ([TransactionId]),
    CONSTRAINT [FK_BorrowingTransactions_AspNetUsers_IssuedByUserId] FOREIGN KEY ([IssuedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_BorrowingTransactions_AspNetUsers_ReturnedToUserId] FOREIGN KEY ([ReturnedToUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_BorrowingTransactions_BookCopies_CopyId] FOREIGN KEY ([CopyId]) REFERENCES [BookCopies] ([CopyId]) ON DELETE CASCADE,
    CONSTRAINT [FK_BorrowingTransactions_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([MemberId]) ON DELETE CASCADE
);
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
    SET IDENTITY_INSERT [AspNetRoles] ON;
INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
VALUES (N'role-admin', NULL, N'Administrator', N'ADMINISTRATOR'),
(N'role-librarian', NULL, N'Librarian', N'LIBRARIAN'),
(N'role-staff', NULL, N'Staff', N'STAFF');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
    SET IDENTITY_INSERT [AspNetRoles] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'AuthorId', N'Bio', N'FirstName', N'LastName') AND [object_id] = OBJECT_ID(N'[Authors]'))
    SET IDENTITY_INSERT [Authors] ON;
INSERT INTO [Authors] ([AuthorId], [Bio], [FirstName], [LastName])
VALUES (1, NULL, N'George', N'Orwell'),
(2, NULL, N'J.K.', N'Rowling'),
(3, NULL, N'Robert C.', N'Martin'),
(4, NULL, N'Martin', N'Fowler'),
(5, NULL, N'Stephen', N'King');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'AuthorId', N'Bio', N'FirstName', N'LastName') AND [object_id] = OBJECT_ID(N'[Authors]'))
    SET IDENTITY_INSERT [Authors] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CategoryId', N'Description', N'Name', N'ParentCategoryId') AND [object_id] = OBJECT_ID(N'[Categories]'))
    SET IDENTITY_INSERT [Categories] ON;
INSERT INTO [Categories] ([CategoryId], [Description], [Name], [ParentCategoryId])
VALUES (1, NULL, N'Fiction', NULL),
(3, NULL, N'Programming', NULL),
(5, NULL, N'History', NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CategoryId', N'Description', N'Name', N'ParentCategoryId') AND [object_id] = OBJECT_ID(N'[Categories]'))
    SET IDENTITY_INSERT [Categories] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PublisherId', N'Email', N'Name', N'Phone') AND [object_id] = OBJECT_ID(N'[Publishers]'))
    SET IDENTITY_INSERT [Publishers] ON;
INSERT INTO [Publishers] ([PublisherId], [Email], [Name], [Phone])
VALUES (1, NULL, N'Penguin Random House', NULL),
(2, NULL, N'HarperCollins', NULL),
(3, NULL, N'O''Reilly Media', NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PublisherId', N'Email', N'Name', N'Phone') AND [object_id] = OBJECT_ID(N'[Publishers]'))
    SET IDENTITY_INSERT [Publishers] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CategoryId', N'Description', N'Name', N'ParentCategoryId') AND [object_id] = OBJECT_ID(N'[Categories]'))
    SET IDENTITY_INSERT [Categories] ON;
INSERT INTO [Categories] ([CategoryId], [Description], [Name], [ParentCategoryId])
VALUES (2, NULL, N'Science Fiction', 1),
(4, NULL, N'Database', 3);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CategoryId', N'Description', N'Name', N'ParentCategoryId') AND [object_id] = OBJECT_ID(N'[Categories]'))
    SET IDENTITY_INSERT [Categories] OFF;
GO


CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO


CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO


CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO


CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO


CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO


CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO


CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO


CREATE INDEX [IX_BookAuthors_BookId] ON [BookAuthors] ([BookId]);
GO


CREATE INDEX [IX_BookCategories_BookId] ON [BookCategories] ([BookId]);
GO


CREATE UNIQUE INDEX [IX_BookCopies_Barcode] ON [BookCopies] ([Barcode]);
GO


CREATE INDEX [IX_BookCopies_BookId] ON [BookCopies] ([BookId]);
GO


CREATE UNIQUE INDEX [IX_Books_ISBN] ON [Books] ([ISBN]);
GO


CREATE INDEX [IX_Books_PublisherId] ON [Books] ([PublisherId]);
GO


CREATE INDEX [IX_BorrowingTransactions_CopyId] ON [BorrowingTransactions] ([CopyId]);
GO


CREATE INDEX [IX_BorrowingTransactions_IssuedByUserId] ON [BorrowingTransactions] ([IssuedByUserId]);
GO


CREATE INDEX [IX_BorrowingTransactions_MemberId] ON [BorrowingTransactions] ([MemberId]);
GO


CREATE INDEX [IX_BorrowingTransactions_ReturnedToUserId] ON [BorrowingTransactions] ([ReturnedToUserId]);
GO


CREATE INDEX [IX_Categories_ParentCategoryId] ON [Categories] ([ParentCategoryId]);
GO


CREATE UNIQUE INDEX [IX_Members_Email] ON [Members] ([Email]);
GO


CREATE UNIQUE INDEX [IX_Members_MembershipNumber] ON [Members] ([MembershipNumber]);
GO


CREATE UNIQUE INDEX [IX_Publishers_Name] ON [Publishers] ([Name]);
GO


CREATE INDEX [IX_UserActivityLogs_UserId] ON [UserActivityLogs] ([UserId]);
GO


