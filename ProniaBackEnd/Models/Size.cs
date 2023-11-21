namespace ProniaBackEnd.Models
{
    public class Size
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProductSize>? ProductSizes { get; set; }
    }
}
