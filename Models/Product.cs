using System.ComponentModel.DataAnnotations;

namespace FormApp.Models
{
    public class Product
    {
        [Display(Name = "Ürün Id")]
        public int ProductId { get; set; }

        [Display(Name = "Ürün Adi")]
        [Required(ErrorMessage = "Gerekli Alan")]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(0, 100000)]
        [Display(Name = "Fiyat")]
        public decimal? Price { get; set; }

        
        [Display(Name = "Resim")]
        public string? Image { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        [Display(Name="Category")]
        [Required] 
        public int? CategoryId { get; set; }

    }
}