using LibraryManagementSystem.Common.Constants;
using LibraryManagementSystem.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


    public DbSet<Book> Books { get; set; }
    public DbSet<BookCopy> BookCopies { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<BookAuthor> BookAuthors { get; set; }
    public DbSet<BookCategory> BookCategories { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<BorrowingTransaction> BorrowingTransactions { get; set; }
    public DbSet<UserActivityLog> UserActivityLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {

        base.OnModelCreating(builder);


        //composite key

        builder.Entity<BookAuthor>()
            .HasKey(ba => new { ba.AuthorId, ba.BookId });

        builder.Entity<BookCategory>()
            .HasKey(bc => new { bc.CategoryId, bc.BookId });

        // onDelete
        builder.Entity<Category>()
            .HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<BorrowingTransaction>()
            .HasOne(t => t.IssuedByUser)
            .WithMany(u => u.IssuedTransactions)
            .HasForeignKey(t => t.IssuedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<BorrowingTransaction>()
            .HasOne(t => t.ReturnedToUser)
            .WithMany(u => u.ReturnedTransactions)
            .HasForeignKey(t => t.ReturnedToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<UserActivityLog>()
            .HasOne(l => l.User)
            .WithMany(u => u.ActivityLogs)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = "role-admin",
                Name = AppRoles.Administrator,
                NormalizedName = AppRoles.Administrator.ToUpper()
            },
            new IdentityRole
            {
                Id = "role-librarian",
                Name = AppRoles.Librarian,
                NormalizedName = AppRoles.Librarian.ToUpper()
            },
            new IdentityRole
            {
                Id = "role-staff",
                Name = AppRoles.Staff,
                NormalizedName = AppRoles.Staff.ToUpper()
            }
        );

        builder.Entity<Publisher>().HasData(
            new Publisher { PublisherId = 1, Name = "Penguin Random House" },
            new Publisher { PublisherId = 2, Name = "HarperCollins" },
            new Publisher { PublisherId = 3, Name = "O'Reilly Media" }
        );

        builder.Entity<Author>().HasData(
            new Author { AuthorId = 1, FirstName = "George", LastName = "Orwell" },
            new Author { AuthorId = 2, FirstName = "J.K.", LastName = "Rowling" },
            new Author { AuthorId = 3, FirstName = "Robert C.", LastName = "Martin" },
            new Author { AuthorId = 4, FirstName = "Martin", LastName = "Fowler" },
            new Author { AuthorId = 5, FirstName = "Stephen", LastName = "King" }
        );

        builder.Entity<Category>().HasData(
            new Category { CategoryId = 1, Name = "Fiction" },
            new Category { CategoryId = 2, Name = "Science Fiction", ParentCategoryId = 1 },
            new Category { CategoryId = 3, Name = "Programming" },
            new Category { CategoryId = 4, Name = "Database", ParentCategoryId = 3 },
            new Category { CategoryId = 5, Name = "History" }
        );
    }
}
