using ProniaBackEnd.Models;
using System.ComponentModel.DataAnnotations;

namespace ProniaBackEnd.Areas.Admin.ViewModels
{
    public class UpdateSizeVM
    {
        
        [Required(ErrorMessage = "Ad mutleq daxil edilmelidir")]
        [MaxLength(25, ErrorMessage = "Uzunlug max 25 olmalidir")]
        public string Name { get; set; }
        
    }
}
