using UTB.Aukce.Application.ViewModels;
using UTB.Aukce.Domain.Entities;

namespace UTB.Aukce.Application.Services
{
    public interface IAuctionService
    {
        Task<IEnumerable<Auction>> GetAllAuctionsAsync();
        Task<Auction?> GetAuctionByIdAsync(int id);
        Task CreateAuctionAsync(AuctionCreateViewModel model);
    }
}