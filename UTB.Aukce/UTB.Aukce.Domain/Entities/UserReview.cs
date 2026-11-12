using System;
using System.Collections.Generic;
using System.Text;

namespace UTB.Aukce.Domain.Entities
{
    public class UserReview
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int AuctionId { get; set; }
        public Auction Auction { get; set; } = null!;
        public string ReviewerId { get; set; } = string.Empty;
        public ApplicationUser Reviewer { get; set; } = null!;

        public string RevieweeId { get; set; } = string.Empty;
        public ApplicationUser Reviewee { get; set; } = null!;
    }
}
