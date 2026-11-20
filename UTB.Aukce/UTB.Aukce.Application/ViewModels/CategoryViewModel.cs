using System.ComponentModel.DataAnnotations;

namespace UTB.Aukce.Application.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Zadejte název kategorie.")]
        [StringLength(100, ErrorMessage = "Název může mít maximálně 100 znaků.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadejte popis kategorie.")]
        [StringLength(500, ErrorMessage = "Popis může mít maximálně 500 znaků.")]
        public string Description { get; set; } = string.Empty;
    }
}