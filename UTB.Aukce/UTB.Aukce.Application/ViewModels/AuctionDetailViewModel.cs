namespace UTB.Aukce.Application.ViewModels
{
    public class AuctionDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string SellerId { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public decimal StartingPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal MinNextBid => CurrentPrice + 1;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsActive => EndTime > DateTime.Now;
        public bool IsWatched { get; set; }
        public bool CanReview { get; set; }

        public List<BidItemViewModel> Bids { get; set; } = new();
        public List<ReviewItemViewModel> Reviews { get; set; } = new();
    }

    public class BidItemViewModel
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string BidderName { get; set; } = string.Empty;
    }

    public class ReviewItemViewModel
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
    }

    public class MyActivityViewModel
    {
        public IEnumerable<AuctionListItemViewModel> MyCreatedAuctions { get; set; } = new List<AuctionListItemViewModel>();
        public IEnumerable<AuctionListItemViewModel> MyBiddedAuctions { get; set; } = new List<AuctionListItemViewModel>();
        public IEnumerable<AuctionListItemViewModel> WatchlistAuctions { get; set; } = new List<AuctionListItemViewModel>();
    }
}