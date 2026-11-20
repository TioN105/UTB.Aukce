using UTB.Aukce.Application.ViewModels;

namespace UTB.Aukce.Application.Services
{
    public interface IAuctionService
    {
        Task<AuctionCatalogViewModel> GetCatalogAsync(int? categoryId, string? searchQuery, bool onlyActive, string? currentUserId);
        Task<AuctionDetailViewModel?> GetAuctionDetailAsync(int id, string? currentUserId);
        Task<MyActivityViewModel> GetMyActivityAsync(string userId);
        Task CreateAuctionAsync(AuctionCreateViewModel model, string sellerId);
        Task<(bool Succeeded, string Error)> PlaceBidAsync(int auctionId, decimal amount, string userId);
        Task ToggleWatchlistAsync(int auctionId, string userId);
        Task<(bool Succeeded, string Error)> AddReviewAsync(int auctionId, int rating, string comment, string reviewerId);
        Task<AdminModerationViewModel> GetAdminModerationDataAsync();
        Task<AdminAuctionEditViewModel?> GetAuctionForAdminEditAsync(int id);
        Task UpdateAuctionAsync(AdminAuctionEditViewModel model);
        Task DeleteAuctionAsync(int id);
        Task DeleteBidAsync(int bidId);
        Task DeleteReviewAsync(int reviewId);
        Task DeleteWatchlistItemAsync(int id);
    }
}