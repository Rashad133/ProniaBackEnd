using ProniaBackEnd.Models;

namespace ProniaBackEnd.ViewModels
{
    public class OrderVM
    {
        public string  Address { get; set; }
        public List<BasketItem>? BasketItems  { get; set; }
    }
}
