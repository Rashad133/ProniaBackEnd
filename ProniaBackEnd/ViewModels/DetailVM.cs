using ProniaBackEnd.Models;

namespace ProniaBackEnd.ViewModels
{
    public class DetailVM
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set;}
    }
}
