using System.ComponentModel.DataAnnotations;

namespace ProniaBackEnd.ViewModels;

public class LoginVM
{
    [Required]
    public string UsernameOrEmail { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    public bool IsRemembered { get; set; }
}
