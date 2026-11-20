using UTB.Aukce.Application.ViewModels;

namespace UTB.Aukce.Application.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryViewModel>> GetAllAsync();
        Task<CategoryViewModel?> GetByIdAsync(int id);
        Task CreateAsync(CategoryViewModel model);
        Task UpdateAsync(CategoryViewModel model);
        Task DeleteAsync(int id);
    }
}