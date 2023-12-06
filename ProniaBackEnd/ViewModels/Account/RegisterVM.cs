using ProniaBackEnd.Utilities.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProniaBackEnd.ViewModels
{
    public class RegisterVM
    {
        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        public string Username { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        public string Name { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        public string Surname { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression("\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*",
        ErrorMessage = "Please enter correct email address")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Select your gender")]
        public Gender Gender { get; set; }
    }
}
