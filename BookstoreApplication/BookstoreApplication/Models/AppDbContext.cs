using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;



namespace BookstoreApplication.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Award> Awards { get; set; }
        public DbSet<AuthorAward> AuthorAwards { get; set; }

        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Award>(cfg =>
            {
                cfg.Property(a => a.Name)
                    .HasMaxLength(200)
                    .IsRequired();

                cfg.Property(a => a.Description)
                    .HasMaxLength(2000)
                    .IsRequired();

                cfg.Property(a => a.StartYear)
                    .IsRequired();

                cfg.ToTable(t => t.HasCheckConstraint("CK_Award_StartYear", "\"StartYear\" >= 1800 AND \"StartYear\" <= EXTRACT(YEAR FROM CURRENT_DATE)"));
            });

            modelBuilder.Entity<Author>(cfg =>
            {
                cfg.Property(a => a.FullName)
                    .HasMaxLength(200)
                    .IsRequired();

                cfg.Property(a => a.Biography)
                    .HasMaxLength(2000)
                    .IsRequired();

                cfg.Property(a => a.DateOfBirth)
                    .HasColumnName("Birthday")
                    .HasColumnType("date");

                cfg.HasIndex(a => a.FullName);
            });

            modelBuilder.Entity<AuthorAward>(cfg =>
            {
                cfg.ToTable("AuthorAwardBridge", t =>
                    t.HasCheckConstraint("CK_AuthorAward_YearAwarded",
                                        "\"YearAwarded\" >= 1800 AND \"YearAwarded\" <= EXTRACT(YEAR FROM CURRENT_DATE)"));

                cfg.HasKey(x => new { x.AuthorId, x.AwardId, x.YearAwarded });

                cfg.HasOne(x => x.Author)
                    .WithMany(a => a.AuthorAwards)
                    .HasForeignKey(a => a.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);

                cfg.HasOne(x => x.Award)
                    .WithMany(a => a.AuthorAwards)
                    .HasForeignKey(x => x.AwardId)
                    .OnDelete(DeleteBehavior.Cascade);

                cfg.Property(x => x.YearAwarded)
                    .IsRequired();
            });

            modelBuilder.Entity<Book>(cfg =>
            {
                cfg.Property(b => b.PublishedDate)
                     .HasColumnType("date");

                cfg.HasOne(b => b.Publisher)
                    .WithMany()
                    .HasForeignKey(b => b.PublisherId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Review>()
                    .HasOne(r => r.Book)
                    .WithMany(b => b.Reviews)
                    .HasForeignKey(r => r.BookId)
                    .OnDelete(DeleteBehavior.Cascade);
            // Seed IdentityRole
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Name = "Bibliotekar", NormalizedName = "BIBLIOTEKAR" },
                new IdentityRole { Name = "Urednik", NormalizedName = "UREDNIK" }
            );
            // === SEED PODACI (v3) ===
            modelBuilder.Entity<Publisher>().HasData(
                new Publisher { Id = 1, Name = "Penguin Books", Address = "80 Strand, London", Website = "https://penguin.co.uk" },
                new Publisher { Id = 2, Name = "Bloomsbury", Address = "50 Bedford Square, London", Website = "https://www.bloomsbury.com" },
                new Publisher { Id = 3, Name = "Vintage Books", Address = "New York, USA", Website = "https://www.vintagebooks.com" }
            );

            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, FullName = "George Orwell", Biography = "British writer and journalist.", DateOfBirth = new DateTime(1903, 6, 25) },
                new Author { Id = 2, FullName = "Jane Austen", Biography = "English novelist known for romantic fiction.", DateOfBirth = new DateTime(1775, 12, 16) },
                new Author { Id = 3, FullName = "J.K. Rowling", Biography = "British author best known for Harry Potter.", DateOfBirth = new DateTime(1965, 7, 31) },
                new Author { Id = 4, FullName = "Mark Twain", Biography = "American writer and humorist.", DateOfBirth = new DateTime(1835, 11, 30) },
                new Author { Id = 5, FullName = "Leo Tolstoy", Biography = "Russian novelist, known for War and Peace.", DateOfBirth = new DateTime(1828, 9, 9) }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "1984", PageCount = 328, PublishedDate = new DateTime(1949, 6, 8), ISBN = "9780451524935", AuthorId = 1, PublisherId = 1 },
                new Book { Id = 2, Title = "Animal Farm", PageCount = 112, PublishedDate = new DateTime(1945, 8, 17), ISBN = "9780451526342", AuthorId = 1, PublisherId = 1 },
                new Book { Id = 3, Title = "Pride and Prejudice", PageCount = 432, PublishedDate = new DateTime(1813, 1, 28), ISBN = "9780141439518", AuthorId = 2, PublisherId = 1 },
                new Book { Id = 4, Title = "Emma", PageCount = 474, PublishedDate = new DateTime(1815, 12, 23), ISBN = "9780141439587", AuthorId = 2, PublisherId = 3 },
                new Book { Id = 5, Title = "Harry Potter and the Philosopher's Stone", PageCount = 223, PublishedDate = new DateTime(1997, 6, 26), ISBN = "9780747532699", AuthorId = 3, PublisherId = 2 },
                new Book { Id = 6, Title = "Harry Potter and the Chamber of Secrets", PageCount = 251, PublishedDate = new DateTime(1998, 7, 2), ISBN = "9780747538493", AuthorId = 3, PublisherId = 2 },
                new Book { Id = 7, Title = "Harry Potter and the Prisoner of Azkaban", PageCount = 317, PublishedDate = new DateTime(1999, 7, 8), ISBN = "9780747542155", AuthorId = 3, PublisherId = 2 },
                new Book { Id = 8, Title = "Adventures of Huckleberry Finn", PageCount = 366, PublishedDate = new DateTime(1884, 12, 10), ISBN = "9780142437179", AuthorId = 4, PublisherId = 3 },
                new Book { Id = 9, Title = "The Adventures of Tom Sawyer", PageCount = 274, PublishedDate = new DateTime(1876, 6, 1), ISBN = "9780143039563", AuthorId = 4, PublisherId = 1 },
                new Book { Id = 10, Title = "War and Peace", PageCount = 1225, PublishedDate = new DateTime(1869, 1, 1), ISBN = "9780140447934", AuthorId = 5, PublisherId = 3 },
                new Book { Id = 11, Title = "Anna Karenina", PageCount = 864, PublishedDate = new DateTime(1877, 4, 1), ISBN = "9780143035008", AuthorId = 5, PublisherId = 3 },
                new Book { Id = 12, Title = "The Death of Ivan Ilyich", PageCount = 86, PublishedDate = new DateTime(1886, 1, 1), ISBN = "9780553210354", AuthorId = 5, PublisherId = 1 }
            );

            modelBuilder.Entity<Award>().HasData(
                new Award { Id = 1, Name = "Nobel Prize in Literature", Description = "Global award for outstanding contributions to literature.", StartYear = 1901 },
                new Award { Id = 2, Name = "Pulitzer Prize for Fiction", Description = "Distinguished fiction by an American author.", StartYear = 1917 },
                new Award { Id = 3, Name = "Booker Prize", Description = "Best original novel in English.", StartYear = 1969 },
                new Award { Id = 4, Name = "National Book Award", Description = "Annual U.S. awards for literature.", StartYear = 1950 }
            );

            // 15 unikatnih dodela (AuthorId, AwardId, YearAwarded) — po 3 dodele za svakog autora
            modelBuilder.Entity<AuthorAward>().HasData(
                // George Orwell (1)
                new AuthorAward { AuthorId = 1, AwardId = 2, YearAwarded = 1949 },
                new AuthorAward { AuthorId = 1, AwardId = 4, YearAwarded = 1951 },
                new AuthorAward { AuthorId = 1, AwardId = 3, YearAwarded = 1970 },

                // Jane Austen (2)
                new AuthorAward { AuthorId = 2, AwardId = 1, YearAwarded = 1905 },
                new AuthorAward { AuthorId = 2, AwardId = 2, YearAwarded = 1925 },
                new AuthorAward { AuthorId = 2, AwardId = 3, YearAwarded = 1971 },

                // J.K. Rowling (3)
                new AuthorAward { AuthorId = 3, AwardId = 4, YearAwarded = 2001 },
                new AuthorAward { AuthorId = 3, AwardId = 2, YearAwarded = 2000 },
                new AuthorAward { AuthorId = 3, AwardId = 3, YearAwarded = 1999 },

                // Mark Twain (4)
                new AuthorAward { AuthorId = 4, AwardId = 1, YearAwarded = 1907 },
                new AuthorAward { AuthorId = 4, AwardId = 2, YearAwarded = 1918 },
                new AuthorAward { AuthorId = 4, AwardId = 4, YearAwarded = 1952 },

                // Leo Tolstoy (5)
                new AuthorAward { AuthorId = 5, AwardId = 1, YearAwarded = 1902 },
                new AuthorAward { AuthorId = 5, AwardId = 2, YearAwarded = 1920 },
                new AuthorAward { AuthorId = 5, AwardId = 3, YearAwarded = 1970 }
            );

        }
    }
}
