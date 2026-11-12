using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace UTB.Aukce.Domain.Entities
{
    public class Auction
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal StartingPrice { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public ICollection<Bid> Bids { get; set; } = new List<Bid>();
        public ICollection<UserReview> Reviews { get; set; } = new List<UserReview>();
        public string SellerId { get; set; } = string.Empty;
        public ApplicationUser Seller { get; set; } = null!;
    }
}