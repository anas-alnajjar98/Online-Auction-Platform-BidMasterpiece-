using Microsoft.EntityFrameworkCore;
using MasterPiece.Models;

namespace MasterPiece.Data
{
    public class AuctionDbContext : DbContext
    {
        public AuctionDbContext(DbContextOptions<AuctionDbContext> options) : base(options) { }

        // Define DbSets for each model (table)
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<OrderHistory> OrderHistories { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Product to Seller relationship (User)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Seller)
                .WithMany()
                .HasForeignKey(p => p.SellerId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete of users causing product deletion

            // Product to Category relationship
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete, so if category is deleted, products are deleted

            // Auction to CurrentHighestBidder relationship (User)
            modelBuilder.Entity<Auction>()
                .HasOne(a => a.CurrentHighestBidder)
                .WithMany()
                .HasForeignKey(a => a.CurrentHighestBidderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Bid to Auction relationship
            modelBuilder.Entity<Bid>()
                .HasOne(b => b.Auction)
                .WithMany(a => a.Bids)
                .HasForeignKey(b => b.AuctionId)
                .OnDelete(DeleteBehavior.Cascade); // If the auction is deleted, related bids are also deleted

            // Bid to User relationship (Bidder)
            modelBuilder.Entity<Bid>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bids)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade); // If the user is deleted, related bids are also deleted

            // Payment to Auction relationship
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Auction)
                .WithMany()
                .HasForeignKey(p => p.AuctionId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete of auction affecting payments

            // Payment to User relationship (User who made the payment)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade); // If the user is deleted, related payments are also deleted

            // OrderHistory relationship (Auction and User)
            modelBuilder.Entity<OrderHistory>()
                .HasOne(oh => oh.Auction)
                .WithMany()
                .HasForeignKey(oh => oh.AuctionId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes to order history

            modelBuilder.Entity<OrderHistory>()
                .HasOne(oh => oh.User)
                .WithMany()
                .HasForeignKey(oh => oh.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Allow cascading if user is deleted

            // Configuring other necessary relationships or constraints...

            base.OnModelCreating(modelBuilder);
        }
    }
}
