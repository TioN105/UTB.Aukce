using System.ComponentModel.DataAnnotations;
using UTB.Aukce.Application.Validation;

namespace UTB.Aukce.Application.ViewModels
{
    public class AuctionCreateViewModel
    {
        [Required(ErrorMessage = "Zadejte název aukce.")]
        [StringLength(100, ErrorMessage = "Název může mít maximálně 100 znaků.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadejte popis aukce.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadejte vyvolávací cenu.")]
        [Range(1, 1000000, ErrorMessage = "Cena musí být mezi 1 a 1 000 000 Kč.")]
        public decimal StartingPrice { get; set; }

        [Required(ErrorMessage = "Zadejte datum a čas konce aukce.")]
        [FutureDate(ErrorMessage = "Datum konce aukce musí být v budoucnosti.")]
        public DateTime EndTime { get; set; } = DateTime.Now.AddDays(7);

        [Required(ErrorMessage = "Vyberte kategorii.")]
        public int CategoryId { get; set; }
    }
}