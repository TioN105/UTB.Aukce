using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using UTB.Aukce.Domain.Entities;

namespace UTB.Aukce.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Tabulky v databázi
        public DbSet<Category> Categories { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<UserReview> UserReviews { get; set; }
        public DbSet<WatchlistItem> WatchlistItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Nastavení Cascade Delete (když se smaže aukce, smažou se s ní i příhozy a recenze)
            modelBuilder.Entity<Bid>()
                .HasOne(b => b.Auction)
                .WithMany(a => a.Bids)
                .HasForeignKey(b => b.AuctionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserReview>()
                .HasOne(r => r.Auction)
                .WithMany(a => a.Reviews)
                .HasForeignKey(r => r.AuctionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WatchlistItem>()
                .HasOne(w => w.Auction)
                .WithMany() // WatchlistItem nemá kolekci uvnitř Auction
                .HasForeignKey(w => w.AuctionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}