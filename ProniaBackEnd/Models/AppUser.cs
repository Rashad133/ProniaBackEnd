using Microsoft.AspNetCore.Identity;
using ProniaBackEnd.Enum;

namespace ProniaBackEnd.Models
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Gender Gender {  get; set; } 
    }
}
