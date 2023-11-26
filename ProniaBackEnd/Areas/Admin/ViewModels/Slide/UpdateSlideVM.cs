using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProniaBackEnd.Areas.Admin.ViewModels
{
    public class UpdateSlideVM
    {
        
        [Required]
        
        public string Title { get; set; }
        public string SubTitle { get; set; }

        public string Description { get; set; }
        public string Image { get; set; }
        public string Order { get; set; }
        

        public IFormFile? Photo { get; set; }
    }
}
