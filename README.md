# Library Management System

A RESTful API built to manage a modern library's operations, including books, members, borrowing transactions, and role-based staff access. It serves as a backend foundation to handle library workflows securely and efficiently.

## Tech Stack

- ASP.NET Core 8.0
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- JWT Authentication
- Swagger

## Architecture

The project follows a clean N-Tier architecture pattern:
- **Controllers:** Handle HTTP routing and request/response formatting, remaining extremely thin.
- **Services:** Contain all business logic and orchestrate database calls. This keeps the controllers clean and makes the business rules testable.
- **EF Core / DbContext:** Manages database access using asynchronous operations and `.AsNoTracking()` for efficient read operations.
- **DTOs:** Used exclusively for API inputs and outputs to prevent exposing the internal database models and to allow precise data shaping.
- **Global Middleware:** Catches exceptions globally and translates them into appropriate HTTP status codes to prevent repetitive `try-catch` blocks in controllers.
- **Identity & JWT:** Secures the API using token-based authentication and role-based authorization.

## Main Design Decisions

- **Books and Book Copies:**
  - A `Book` represents the shared metadata (Title, ISBN, Authors, Categories, Publisher).
  - A `BookCopy` represents a specific physical item on the shelf (Barcode, Status, associated Book).
  - *Why:* A library often holds multiple physical copies of the same book. This separation allows each individual copy to independently be marked as "Available", "Borrowed", or "Lost".

- **Borrowing:**
  - Borrowing transactions are tied strictly to a `BookCopy`, not a general `Book`. When a physical copy is checked out, its specific status updates to `Borrowed`. Returning the specific copy switches its status back to `Available`.

- **Categories:**
  - Categories are designed hierarchically using a self-referencing `ParentCategoryId`.
  - Example:
    ```
    Fiction
     ├── Fantasy
     └── Mystery
    ```

- **Authentication and Authorization:**
  - ASP.NET Core Identity manages user accounts and secure password hashing.
  - JWT (JSON Web Tokens) are used to authenticate requests statelessly.
  - Role-based Access Control (RBAC) enforces permissions across three tiers:
    - **Administrator:** Full system access.
    - **Librarian:** Can manage books, categories, and inventory.
    - **Staff:** Can manage members and process borrowing/returns.

- **Soft Delete / Deactivation:**
  - Books use a boolean `IsDeleted` flag to hide them from queries without breaking historical borrowing records.
  - Members use a `Status` string (Active/Inactive) to suspend privileges without deleting the user.

- **Activity Logging:**
  - Important administrative actions (e.g., creating books or users) are logged in the database to maintain an audit trail.
  - This logic is centralized in an `IActivityLogService` to avoid duplicating EF Core code across different services.

- **DTOs:**
  - DTOs are used to decouple the API contract from the database schema. They prevent over-posting vulnerabilities and allow us to project complex relational data into flat, easy-to-read JSON responses.

## API Overview

| Area | Examples |
|------|----------|
| Authentication | `POST /api/auth/login` |
| Books | `GET /api/books`, `POST /api/books`, `PATCH /api/books/{id}` |
| Book Copies | `POST /api/bookcopies`, `GET /api/bookcopies/book/{bookId}` |
| Members | `GET /api/members`, `POST /api/members`, `PATCH /api/members/{id}` |
| Borrowing | `POST /api/borrowing/borrow`, `POST /api/borrowing/{id}/return` |
| Users | `GET /api/users`, `POST /api/users` |
| Categories | `GET /api/categories`, `POST /api/categories` |
| Activity Logs | `GET /api/activitylogs` |

## Error Handling

The application relies on a `GlobalExceptionHandlingMiddleware`. Services throw domain-specific exceptions (e.g., `NotFoundException` when an entity doesn't exist, or `ConflictException` for duplicate barcodes). The middleware intercepts these and automatically translates them into standard HTTP responses (`404 Not Found`, `409 Conflict`), keeping the controllers completely free of manual error handling logic.

## Database Design

The schema leverages relational design principles:
- **Book → BookCopies:** 1-to-Many
- **Book ↔ Authors:** Many-to-Many (via `BookAuthor` junction table)
- **Book ↔ Categories:** Many-to-Many (via `BookCategory` junction table)
- **Publisher → Books:** 1-to-Many
- **Member → BorrowingTransactions:** 1-to-Many
- **BookCopy → BorrowingTransactions:** 1-to-Many
- **ApplicationUser → ActivityLogs:** 1-to-Many

## Sample Data

For ease of testing, the system automatically seeds sample data into the database upon startup, including:
- **Roles & Users:** Three pre-configured staff accounts (Admin, Librarian, Staff).
- **Lookup Data:** A starter set of Publishers, Authors, and hierarchical Categories.
- **Inventory:** An initial book (`Clean Code`) with an available physical copy.
- **Members:** A sample library member (`John Doe`).

## Running the Project

1. Update the SQL Server connection string in `appsettings.json` if necessary.
2. Apply the EF Core database migrations:
   ```bash
   dotnet ef database update
   ```
3. Run the application:
   ```bash
   dotnet run
   ```
4. Navigate to the Swagger UI in your browser (e.g., `http://localhost:5239/swagger`).
5. Use the `POST /api/auth/login` endpoint with `admin` / `Admin123!` to retrieve a JWT.
6. Click the "Authorize" button in Swagger and paste your token (prefix with `Bearer `) to test secured endpoints. (Alternatively, import the included Postman collection).

## Testing Flow

To evaluate the system quickly, we recommend this sequence:
1. **Login:** Authenticate as `admin` to receive your token.
2. **Setup:** Fetch existing Categories and Authors.
3. **Inventory:** Create a new Book, then add a Book Copy using the Book's ID.
4. **Members:** Create a new Member.
5. **Checkout:** Borrow the newly created copy using the Member's ID.
6. **Verification:** Check the Book Copies endpoint to verify the copy status is now `Borrowed`.
7. **Return:** Return the copy and verify the status flips back to `Available`.
8. **Audit:** Check the Activity Logs endpoint to see your actions recorded.

## Design Notes / Trade-offs

- **Service Layer vs Repository Pattern:** A service layer handles the business logic, but the `DbContext` is injected directly into services rather than abstracting it behind a Repository pattern. This prevents unnecessary boilerplate since EF Core is already an abstraction of the Unit of Work and Repository patterns.
- **Separation of Book and BookCopy:** Adds slight complexity to borrowing, but is strictly necessary because real libraries manage inventory at the physical copy level, not the title level.
- **Stateless Authentication:** JWTs are used instead of session cookies to ensure the API remains stateless and easily consumable by mobile or SPA clients.

## Notes

This project was intentionally designed to strike a balance between demonstrating solid ASP.NET Core principles (N-Tier architecture, DTOs, Global Middleware) while avoiding over-engineering (no MediatR, CQRS, or generic repository boilerplate). It focuses on writing clean, readable, and highly maintainable code.
