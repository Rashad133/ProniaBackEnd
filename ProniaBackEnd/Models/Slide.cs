using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProniaBackEnd.Models
{
    public class Slide
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(25, ErrorMessage = "Uzunlugu max 30 olmalidir")]
        public string Title { get; set; }
        public string SubTitle { get; set; }

        public string Description { get; set; }
        public string Image { get; set; }
        public string Order { get; set; }
        [NotMapped]

        public IFormFile? Photo { get; set; }

    }
}
