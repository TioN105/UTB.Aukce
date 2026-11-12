using UTB.Aukce.Application.ViewModels;
using UTB.Aukce.Domain.Entities;
using UTB.Aukce.Domain.Interfaces;

namespace UTB.Aukce.Application.Services
{
    public class AuctionService : IAuctionService
    {
        private readonly IRepository<Auction> _auctionRepository;

        public AuctionService(IRepository<Auction> auctionRepository)
        {
            _auctionRepository = auctionRepository;
        }

        public async Task<IEnumerable<Auction>> GetAllAuctionsAsync()
        {
            return await _auctionRepository.GetAllAsync();
        }

        public async Task<Auction?> GetAuctionByIdAsync(int id)
        {
            return await _auctionRepository.GetByIdAsync(id);
        }

        public async Task CreateAuctionAsync(AuctionCreateViewModel model)
        {
            var auction = new Auction
            {
                Title = model.Title,
                Description = model.Description,
                StartingPrice = model.StartingPrice,
                StartTime = DateTime.Now,
                EndTime = model.EndTime,
                CategoryId = model.CategoryId
            };

            await _auctionRepository.AddAsync(auction);
            await _auctionRepository.SaveChangesAsync();
        }
    }
}