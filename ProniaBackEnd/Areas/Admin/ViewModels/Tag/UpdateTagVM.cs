using ProniaBackEnd.Models;
using System.ComponentModel.DataAnnotations;

namespace ProniaBackEnd.Areas.Admin.ViewModels
{
    public class UpdateTagVM
    {
        
        [Required(ErrorMessage = "Ad daxil edin")]
        [MaxLength(25, ErrorMessage = "Uzunlugu max 25 olmalidir")]
        public string Name { get; set; }
        
    }
}
