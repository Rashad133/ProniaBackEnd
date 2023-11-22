using System.ComponentModel.DataAnnotations;

namespace ProniaBackEnd.Models
{
    public class Tag
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Ad daxil edin")]
        [MaxLength(25,ErrorMessage ="Uzunlugu max 25 olamlidir")]
        public string Name { get; set; }
        public List<ProductTag>? ProductTags { get; set; } 

    }
}
