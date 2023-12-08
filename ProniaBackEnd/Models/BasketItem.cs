namespace ProniaBackEnd.Models
{
    public class BasketItem
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public int? OrderId { get; set; }
        public Order Order { get; set; }
    }
}
