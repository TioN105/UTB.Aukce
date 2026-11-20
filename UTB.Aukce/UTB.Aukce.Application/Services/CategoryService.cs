using UTB.Aukce.Application.ViewModels;
using UTB.Aukce.Domain.Entities;
using UTB.Aukce.Domain.Interfaces;

namespace UTB.Aukce.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryService(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryViewModel>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            });
        }

        public async Task<CategoryViewModel?> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;

            return new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task CreateAsync(CategoryViewModel model)
        {
            var category = new Category
            {
                Name = model.Name,
                Description = model.Description
            };

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(CategoryViewModel model)
        {
            var category = await _categoryRepository.GetByIdAsync(model.Id);
            if (category == null) return;

            category.Name = model.Name;
            category.Description = model.Description;

            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return;

            _categoryRepository.Delete(category);
            await _categoryRepository.SaveChangesAsync();
        }
    }
}