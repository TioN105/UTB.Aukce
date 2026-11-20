using System.ComponentModel.DataAnnotations;

namespace UTB.Aukce.Application.ViewModels
{
    public class AdminAuctionEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Zadejte název aukce.")]
        [StringLength(100, ErrorMessage = "Název může mít maximálně 100 znaků.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadejte popis aukce.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadejte vyvolávací cenu.")]
        [Range(1, 1000000, ErrorMessage = "Cena musí být mezi 1 a 1 000 000 Kč.")]
        public decimal StartingPrice { get; set; }

        [Required(ErrorMessage = "Zadejte datum a čas konce aukce.")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "Vyberte kategorii.")]
        public int CategoryId { get; set; }

        public string SellerName { get; set; } = string.Empty;
        public IEnumerable<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
    }

    public class AdminBidViewModel
    {
        public int Id { get; set; }
        public int AuctionId { get; set; }
        public string AuctionTitle { get; set; } = string.Empty;
        public string BidderName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AdminReviewViewModel
    {
        public int Id { get; set; }
        public int AuctionId { get; set; }
        public string AuctionTitle { get; set; } = string.Empty;
        public string ReviewerName { get; set; } = string.Empty;
        public string RevieweeName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class AdminModerationViewModel
    {
        public IEnumerable<AuctionListItemViewModel> Auctions { get; set; } = new List<AuctionListItemViewModel>();
        public IEnumerable<AdminBidViewModel> Bids { get; set; } = new List<AdminBidViewModel>();
        public IEnumerable<AdminReviewViewModel> Reviews { get; set; } = new List<AdminReviewViewModel>();
        public IEnumerable<AdminWatchlistViewModel> WatchlistItems { get; set; } = new List<AdminWatchlistViewModel>();
    }
    public class AdminWatchlistViewModel
    {
        public int Id { get; set; }
        public string AuctionTitle { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime AddedAt { get; set; }
    }
}