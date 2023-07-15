using Microsoft.EntityFrameworkCore;
using PublisherDomain;

namespace PublisherData;

public class PubContext : DbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
                "Server=localhost;Database=PubDatabase;User Id=sa;Password=Password123!;TrustServerCertificate=True")
            .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name })
            .EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>().HasData(
            new Author { AuthorId = 1, FirstName = "Rhoda", LastName = "Lerman" });

        var authorList = new Author[]
        {
            new Author { AuthorId = 2, FirstName = "Ruth", LastName = "Ozeki" },
            new Author { AuthorId = 3, FirstName = "Sofia", LastName = "Segovia" },
            new Author { AuthorId = 4, FirstName = "Ursula K.", LastName = "LeGuin" },
            new Author { AuthorId = 5, FirstName = "Hugh", LastName = "Howey" },
            new Author { AuthorId = 6, FirstName = "Isabelle", LastName = "Allende" },
        };
        modelBuilder.Entity<Author>().HasData(authorList);

        var someBooks = new Book[]
        {
            new Book { BookId = 1, AuthorFK = 1, Title = "In God's Ear", PublishDate = new DateTime(1989, 3, 1) },
            new Book
            {
                BookId = 2, AuthorFK = 2, Title = "A Tale For the Time Being", PublishDate = new DateTime(2013, 12, 31)
            },
            new Book
            {
                BookId = 3, AuthorFK = 3, Title = "The Left Hand of Darkness", PublishDate = new DateTime(1969, 3, 1)
            }
        };
        modelBuilder.Entity<Book>().HasData(someBooks);

        modelBuilder.Entity<Author>().HasMany(a => a.Books).WithOne(b => b.Author).HasForeignKey(b => b.AuthorFK);
    }
}