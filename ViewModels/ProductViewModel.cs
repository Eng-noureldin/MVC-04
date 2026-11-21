using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Session04_MVC.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        [StringLength(100, MinimumLength = 3)]
        [Display(Name = "Product Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 100000.00)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Stock quantity is required.")]
        [Range(0, 10000)]
        [Display(Name = "Stock Quantity")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Please select a Category.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        // Image file for upload
        [Display(Name = "Product Image")]
        public IFormFile? Image { get; set; }

        public string? ExistingImageName { get; set; }

        public IEnumerable<SelectListItem>? AvailableCategories { get; set; }
    }
}
