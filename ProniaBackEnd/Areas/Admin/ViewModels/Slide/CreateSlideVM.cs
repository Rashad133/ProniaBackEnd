using System.ComponentModel.DataAnnotations;

namespace ProniaBackEnd.Areas.Admin.ViewModels
{
    public class CreateSlideVM
    {
        
        [Required]
        
        public string Title { get; set; }
        public string SubTitle { get; set; }

        public string Description { get; set; }
        public string Image { get; set; }
        public string Order { get; set; }
        [Required]
        
        public IFormFile? Photo { get; set; }
    }
}
