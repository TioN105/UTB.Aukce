using UTB.Aukce.Application.ViewModels;
using UTB.Aukce.Domain.Entities;
using UTB.Aukce.Domain.Interfaces;

namespace UTB.Aukce.Application.Services
{
    public class AuctionService : IAuctionService
    {
        private readonly IRepository<Auction> _auctionRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Bid> _bidRepository;
        private readonly IRepository<WatchlistItem> _watchlistRepository;
        private readonly IRepository<UserReview> _reviewRepository;

        public AuctionService(
            IRepository<Auction> auctionRepository,
            IRepository<Category> categoryRepository,
            IRepository<Bid> bidRepository,
            IRepository<WatchlistItem> watchlistRepository,
            IRepository<UserReview> reviewRepository)
        {
            _auctionRepository = auctionRepository;
            _categoryRepository = categoryRepository;
            _bidRepository = bidRepository;
            _watchlistRepository = watchlistRepository;
            _reviewRepository = reviewRepository;
        }

        public async Task<AuctionCatalogViewModel> GetCatalogAsync(int? categoryId, string? searchQuery, bool onlyActive, string? currentUserId)
        {
            var auctions = await _auctionRepository.GetAllIncludingAsync(a => a.Category, a => a.Seller, a => a.Bids);
            var categories = await _categoryRepository.GetAllAsync();
            var watchedIds = await GetWatchedAuctionIdsAsync(currentUserId);

            var filtered = auctions.AsEnumerable();

            if (onlyActive)
                filtered = filtered.Where(a => a.EndTime > DateTime.Now);

            if (categoryId.HasValue && categoryId.Value > 0)
                filtered = filtered.Where(a => a.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var q = searchQuery.Trim();
                filtered = filtered.Where(a =>
                    a.Title.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    a.Description.Contains(q, StringComparison.OrdinalIgnoreCase));
            }

            var items = filtered
                .OrderBy(a => a.EndTime)
                .Select(a => MapToListItem(a, watchedIds))
                .ToList();

            return new AuctionCatalogViewModel
            {
                Auctions = items,
                Categories = categories.Select(c => new CategoryViewModel { Id = c.Id, Name = c.Name, Description = c.Description }),
                SelectedCategoryId = categoryId,
                SearchQuery = searchQuery,
                OnlyActive = onlyActive
            };
        }

        public async Task<AuctionDetailViewModel?> GetAuctionDetailAsync(int id, string? currentUserId)
        {
            var auctions = await _auctionRepository.GetAllIncludingAsync(a => a.Category, a => a.Seller);
            var auction = auctions.FirstOrDefault(a => a.Id == id);
            if (auction == null) return null;

            var allBids = await _bidRepository.GetAllIncludingAsync(b => b.User);
            var auctionBids = allBids
                .Where(b => b.AuctionId == id)
                .OrderByDescending(b => b.Amount)
                .ThenByDescending(b => b.CreatedAt)
                .ToList();

            var allReviews = await _reviewRepository.GetAllIncludingAsync(r => r.Reviewer);
            var auctionReviews = allReviews
                .Where(r => r.AuctionId == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            var watchedIds = await GetWatchedAuctionIdsAsync(currentUserId);
            var currentPrice = auctionBids.Any() ? auctionBids.Max(b => b.Amount) : auction.StartingPrice;

            bool canReview = !string.IsNullOrEmpty(currentUserId)
                && currentUserId != auction.SellerId
                && !auctionReviews.Any(r => r.ReviewerId == currentUserId);

            return new AuctionDetailViewModel
            {
                Id = auction.Id,
                Title = auction.Title,
                Description = auction.Description,
                CategoryName = auction.Category?.Name ?? "Bez kategorie",
                SellerId = auction.SellerId,
                SellerName = FormatUserName(auction.Seller),
                StartingPrice = auction.StartingPrice,
                CurrentPrice = currentPrice,
                StartTime = auction.StartTime,
                EndTime = auction.EndTime,
                IsWatched = watchedIds.Contains(auction.Id),
                CanReview = canReview,
                Bids = auctionBids.Select(b => new BidItemViewModel
                {
                    Id = b.Id,
                    Amount = b.Amount,
                    CreatedAt = b.CreatedAt,
                    BidderName = FormatUserName(b.User)
                }).ToList(),
                Reviews = auctionReviews.Select(r => new ReviewItemViewModel
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    ReviewerName = FormatUserName(r.Reviewer)
                }).ToList()
            };
        }

        public async Task<MyActivityViewModel> GetMyActivityAsync(string userId)
        {
            var auctions = await _auctionRepository.GetAllIncludingAsync(a => a.Category, a => a.Seller, a => a.Bids);
            var watchedIds = await GetWatchedAuctionIdsAsync(userId);

            return new MyActivityViewModel
            {
                MyCreatedAuctions = auctions
                    .Where(a => a.SellerId == userId)
                    .OrderByDescending(a => a.StartTime)
                    .Select(a => MapToListItem(a, watchedIds))
                    .ToList(),
                MyBiddedAuctions = auctions
                    .Where(a => a.Bids.Any(b => b.UserId == userId))
                    .OrderBy(a => a.EndTime)
                    .Select(a => MapToListItem(a, watchedIds))
                    .ToList(),
                WatchlistAuctions = auctions
                    .Where(a => watchedIds.Contains(a.Id))
                    .OrderBy(a => a.EndTime)
                    .Select(a => MapToListItem(a, watchedIds))
                    .ToList()
            };
        }

        public async Task CreateAuctionAsync(AuctionCreateViewModel model, string sellerId)
        {
            var auction = new Auction
            {
                Title = model.Title,
                Description = model.Description,
                StartingPrice = model.StartingPrice,
                StartTime = DateTime.Now,
                EndTime = model.EndTime,
                CategoryId = model.CategoryId,
                SellerId = sellerId
            };

            await _auctionRepository.AddAsync(auction);
            await _auctionRepository.SaveChangesAsync();
        }

        public async Task<(bool Succeeded, string Error)> PlaceBidAsync(int auctionId, decimal amount, string userId)
        {
            var auctions = await _auctionRepository.GetAllIncludingAsync(a => a.Bids);
            var auction = auctions.FirstOrDefault(a => a.Id == auctionId);

            if (auction == null)
                return (false, "Aukce nebyla nalezena.");

            if (auction.EndTime <= DateTime.Now)
                return (false, "Tato aukce již skončila.");

            if (auction.SellerId == userId)
                return (false, "Nemůžete přihazovat na svou vlastní aukci.");

            var currentPrice = auction.Bids.Any() ? auction.Bids.Max(b => b.Amount) : auction.StartingPrice;
            if (amount <= currentPrice)
                return (false, $"Příhoz musí být vyšší než aktuální cena ({currentPrice:N0} Kč).");

            var bid = new Bid
            {
                AuctionId = auctionId,
                UserId = userId,
                Amount = amount,
                CreatedAt = DateTime.Now
            };

            await _bidRepository.AddAsync(bid);
            await _bidRepository.SaveChangesAsync();

            return (true, string.Empty);
        }

        public async Task ToggleWatchlistAsync(int auctionId, string userId)
        {
            var items = await _watchlistRepository.GetAllAsync();
            var existing = items.FirstOrDefault(w => w.AuctionId == auctionId && w.UserId == userId);

            if (existing != null)
            {
                _watchlistRepository.Delete(existing);
            }
            else
            {
                await _watchlistRepository.AddAsync(new WatchlistItem
                {
                    AuctionId = auctionId,
                    UserId = userId,
                    AddedAt = DateTime.Now
                });
            }

            await _watchlistRepository.SaveChangesAsync();
        }

        public async Task<(bool Succeeded, string Error)> AddReviewAsync(int auctionId, int rating, string comment, string reviewerId)
        {
            var auction = await _auctionRepository.GetByIdAsync(auctionId);
            if (auction == null)
                return (false, "Aukce nebyla nalezena.");

            if (auction.SellerId == reviewerId)
                return (false, "Nemůžete hodnotit sami sebe.");

            if (rating < 1 || rating > 5)
                return (false, "Hodnocení musí být v rozsahu 1 až 5 hvězdiček.");

            var reviews = await _reviewRepository.GetAllAsync();
            if (reviews.Any(r => r.AuctionId == auctionId && r.ReviewerId == reviewerId))
                return (false, "Tuto aukci jste již hodnotil(a).");

            var review = new UserReview
            {
                AuctionId = auctionId,
                ReviewerId = reviewerId,
                RevieweeId = auction.SellerId,
                Rating = rating,
                Comment = comment?.Trim() ?? string.Empty,
                CreatedAt = DateTime.Now
            };

            await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveChangesAsync();

            return (true, string.Empty);
        }
        public async Task<AdminModerationViewModel> GetAdminModerationDataAsync()
        {
            var auctions = await _auctionRepository.GetAllIncludingAsync(a => a.Category, a => a.Seller, a => a.Bids);
            var bids = await _bidRepository.GetAllIncludingAsync(b => b.Auction, b => b.User);
            var reviews = await _reviewRepository.GetAllIncludingAsync(r => r.Auction, r => r.Reviewer, r => r.Reviewee);
            var watchlist = await _watchlistRepository.GetAllIncludingAsync(w => w.Auction, w => w.User);
            var emptyWatchlist = new HashSet<int>();

            return new AdminModerationViewModel
            {
                Auctions = auctions
                    .OrderByDescending(a => a.StartTime)
                    .Select(a => MapToListItem(a, emptyWatchlist))
                    .ToList(),
                Bids = bids
                    .OrderByDescending(b => b.CreatedAt)
                    .Select(b => new AdminBidViewModel
                    {
                        Id = b.Id,
                        AuctionId = b.AuctionId,
                        AuctionTitle = b.Auction?.Title ?? "Neznámá aukce",
                        BidderName = FormatUserName(b.User),
                        Amount = b.Amount,
                        CreatedAt = b.CreatedAt
                    }).ToList(),
                Reviews = reviews
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new AdminReviewViewModel
                    {
                        Id = r.Id,
                        AuctionId = r.AuctionId,
                        AuctionTitle = r.Auction?.Title ?? "Neznámá aukce",
                        ReviewerName = FormatUserName(r.Reviewer),
                        RevieweeName = FormatUserName(r.Reviewee),
                        Rating = r.Rating,
                        Comment = r.Comment,
                        CreatedAt = r.CreatedAt
                    }).ToList(),
                WatchlistItems = watchlist
                    .OrderByDescending(w => w.AddedAt)
                    .Select(w => new AdminWatchlistViewModel
                    {
                        Id = w.Id,
                        AuctionTitle = w.Auction?.Title ?? "Neznámá aukce",
                        UserName = FormatUserName(w.User),
                        AddedAt = w.AddedAt
                    }).ToList()
            };
        }

        public async Task<AdminAuctionEditViewModel?> GetAuctionForAdminEditAsync(int id)
        {
            var auctions = await _auctionRepository.GetAllIncludingAsync(a => a.Seller);
            var auction = auctions.FirstOrDefault(a => a.Id == id);
            if (auction == null) return null;

            var categories = await _categoryRepository.GetAllAsync();

            return new AdminAuctionEditViewModel
            {
                Id = auction.Id,
                Title = auction.Title,
                Description = auction.Description,
                StartingPrice = auction.StartingPrice,
                EndTime = auction.EndTime,
                CategoryId = auction.CategoryId,
                SellerName = FormatUserName(auction.Seller),
                Categories = categories.Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
            };
        }

        public async Task UpdateAuctionAsync(AdminAuctionEditViewModel model)
        {
            var auction = await _auctionRepository.GetByIdAsync(model.Id);
            if (auction == null) return;

            auction.Title = model.Title;
            auction.Description = model.Description;
            auction.StartingPrice = model.StartingPrice;
            auction.EndTime = model.EndTime;
            auction.CategoryId = model.CategoryId;

            _auctionRepository.Update(auction);
            await _auctionRepository.SaveChangesAsync();
        }

        public async Task DeleteAuctionAsync(int id)
        {
            var auction = await _auctionRepository.GetByIdAsync(id);
            if (auction == null) return;

            var bids = (await _bidRepository.GetAllAsync()).Where(b => b.AuctionId == id);
            foreach (var bid in bids) _bidRepository.Delete(bid);

            var reviews = (await _reviewRepository.GetAllAsync()).Where(r => r.AuctionId == id);
            foreach (var rev in reviews) _reviewRepository.Delete(rev);

            var watched = (await _watchlistRepository.GetAllAsync()).Where(w => w.AuctionId == id);
            foreach (var item in watched) _watchlistRepository.Delete(item);

            _auctionRepository.Delete(auction);
            await _auctionRepository.SaveChangesAsync();
        }

        public async Task DeleteBidAsync(int bidId)
        {
            var bid = await _bidRepository.GetByIdAsync(bidId);
            if (bid == null) return;

            _bidRepository.Delete(bid);
            await _bidRepository.SaveChangesAsync();
        }

        public async Task DeleteReviewAsync(int reviewId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null) return;

            _reviewRepository.Delete(review);
            await _reviewRepository.SaveChangesAsync();
        }
        public async Task DeleteWatchlistItemAsync(int id)
        {
            var item = await _watchlistRepository.GetByIdAsync(id);
            if (item == null) return;

            _watchlistRepository.Delete(item);
            await _watchlistRepository.SaveChangesAsync();
        }
        private async Task<HashSet<int>> GetWatchedAuctionIdsAsync(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new HashSet<int>();

            var items = await _watchlistRepository.GetAllAsync();
            return items.Where(w => w.UserId == userId).Select(w => w.AuctionId).ToHashSet();
        }

        private static AuctionListItemViewModel MapToListItem(Auction a, HashSet<int> watchedIds)
        {
            var currentPrice = a.Bids.Any() ? a.Bids.Max(b => b.Amount) : a.StartingPrice;
            return new AuctionListItemViewModel
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                CategoryName = a.Category?.Name ?? "Bez kategorie",
                SellerName = FormatUserName(a.Seller),
                StartingPrice = a.StartingPrice,
                CurrentPrice = currentPrice,
                BidCount = a.Bids.Count,
                EndTime = a.EndTime,
                IsWatched = watchedIds.Contains(a.Id)
            };
        }

        private static string FormatUserName(ApplicationUser? user)
        {
            if (user == null) return "Neznámý uživatel";
            var fullName = $"{user.FirstName} {user.LastName}".Trim();
            return string.IsNullOrEmpty(fullName) ? (user.Email ?? "Uživatel") : fullName;
        }
    }
}