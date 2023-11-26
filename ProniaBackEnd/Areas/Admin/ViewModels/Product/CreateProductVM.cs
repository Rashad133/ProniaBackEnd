using ProniaBackEnd.Models;

namespace ProniaBackEnd.Areas.Admin.ViewModels
{
    public class CreateProductVM
    {
       
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string SKU { get; set; }
        public int? CategoryId { get; set; }
        
    }
}
