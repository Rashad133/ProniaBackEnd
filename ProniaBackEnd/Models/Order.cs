namespace ProniaBackEnd.Models
{
    public class Order
    {
        public int Id { get; set; } 
        public List<BasketItem> BasketItems { get; set; }
    }
}
