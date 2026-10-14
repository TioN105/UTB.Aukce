using System;
using System.Collections.Generic;
using System.Text;

namespace UTB.Aukce.Domain.Entities
{
    public class WatchlistItem
    {
        public int Id { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public int AuctionId { get; set; }
        public Auction Auction { get; set; } = null!;

        // Po přidání Identnity, přidáme userid
    }
}
