using System.ComponentModel.DataAnnotations;

namespace ProniaBackEnd.Models
{
    public class Color
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Ad mutleq daxil edilmelidir")]
        [MaxLength(25, ErrorMessage = "Uzunlug max 25 olmalidir")]
        public string Name { get; set; }
        public List<ProductColor>? ProductColors { get; set; }
        
    }
}
