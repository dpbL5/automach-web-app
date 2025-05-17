using Microsoft.EntityFrameworkCore;
using automach_backend.Models;

namespace automach_backend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameOwned> GamesOwned { get; set; }
        public DbSet<GameTag> GameTags { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionItem> TransactionItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Config cho GameOwned
            // Khai bao khoa chinh cho bang GameOwned
            // Khoa chinh la ket hop giua AccountId va GameId
            // Khai bao quan he giua GameOwned va Game
            modelBuilder.Entity<GameOwned>()
                .HasKey(go => new { go.GameId, go.AccountId });
            modelBuilder.Entity<GameOwned>()
                .HasOne(g => g.Game)
                .WithMany(go => go.GamesOwned)
                .HasForeignKey(g => g.GameId);
            modelBuilder.Entity<GameOwned>()
                .HasOne(a => a.Account)
                .WithMany(go => go.GamesOwned)
                .HasForeignKey(a => a.AccountId);

            // Config cho GameTag
            modelBuilder.Entity<GameTag>()
                .HasKey(gt => new { gt.GameId, gt.TagId });
            modelBuilder.Entity<GameTag>()
                .HasOne(g => g.Game)
                .WithMany(gt => gt.GameTags)
                .HasForeignKey(g => g.GameId);
            modelBuilder.Entity<GameTag>()
                .HasOne(t => t.Tag)
                .WithMany(gt => gt.GameTags)
                .HasForeignKey(t => t.TagId);

            // Config cho TransactionItem
            modelBuilder.Entity<TransactionItem>()
                .HasKey(ti => new { ti.TransactionId, ti.GameId });
            modelBuilder.Entity<TransactionItem>()
                .HasOne(t => t.Transaction)
                .WithMany(ti => ti.TransactionItems)
                .HasForeignKey(t => t.TransactionId);
            modelBuilder.Entity<TransactionItem>()
                .HasOne(g => g.Game)
                .WithMany(ti => ti.TransactionItems)
                .HasForeignKey(g => g.GameId);

            // Config cho Review
            modelBuilder.Entity<Review>()
                .HasKey(r => new { r.GameId, r.AccountId, r.Id });
            modelBuilder.Entity<Review>()
                .HasOne(g => g.Game)
                .WithMany(r => r.Reviews)
                .HasForeignKey(g => g.GameId);
            modelBuilder.Entity<Review>()
                .HasOne(a => a.Account)
                .WithMany(r => r.Reviews)
                .HasForeignKey(a => a.AccountId);

            // Config cho Transaction
            modelBuilder.Entity<Transaction>()
                .HasKey(t => t.Id);
            modelBuilder.Entity<Transaction>()
                .HasOne(a => a.Account)
                .WithMany(t => t.Transactions)
                .HasForeignKey(a => a.AccountId);


            // Seed data
            // modelBuilder.Entity<Tag>().HasData(
            //     new Tag { Id = 1, Title = "Action" },
            //     new Tag { Id = 2, Title = "Adventure" },
            //     new Tag { Id = 4, Title = "Strategy" },
            //     new Tag { Id = 5, Title = "Simulation" },
            //     new Tag { Id = 3, Title = "RPG" }
            // );
            // modelBuilder.Entity<Game>().HasData(
            //     new Game { Id = 1, Title = "Counter-Strike 2", Price = 0.0f },
            //     new Game { Id = 2, Title = "Elden Ring", Price = 59.99f }
            // );

            // modelBuilder.Entity<Account>().HasData(
            //     new Account
            //     {
            //         Id = 1,
            //         Username = "admin",
            //         Password = "123456",
            //         Email = "admin@automach.com",
            //         PhoneNumber = "0123456789",
            //         Name = "Admin",
            //         CreatedAt = DateTime.Today,
            //         Role = "admin"
            //     }
            // );
        }
    }
}