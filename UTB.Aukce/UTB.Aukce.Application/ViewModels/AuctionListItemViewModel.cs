namespace UTB.Aukce.Application.ViewModels
{
    public class AuctionListItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public decimal StartingPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public int BidCount { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsActive => EndTime > DateTime.Now;
        public bool IsWatched { get; set; }
    }

    public class AuctionCatalogViewModel
    {
        public IEnumerable<AuctionListItemViewModel> Auctions { get; set; } = new List<AuctionListItemViewModel>();
        public IEnumerable<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
        public int? SelectedCategoryId { get; set; }
        public string? SearchQuery { get; set; }
        public bool OnlyActive { get; set; } = true;
    }
}