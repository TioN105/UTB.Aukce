using System;
using System.Collections.Generic;
using System.Text;

namespace UTB.Aukce.Domain.Entities
{
    public class Bid
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int AuctionId { get; set; }
        public Auction Auction { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

    }
}
