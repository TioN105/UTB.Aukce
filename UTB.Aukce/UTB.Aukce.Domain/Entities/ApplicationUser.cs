using Microsoft.AspNetCore.Identity;

namespace UTB.Aukce.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        public ICollection<Auction> Auctions { get; set; } = new List<Auction>();
        public ICollection<Bid> Bids { get; set; } = new List<Bid>();
        public ICollection<WatchlistItem> WatchlistItems { get; set; } = new List<WatchlistItem>();
    }
}