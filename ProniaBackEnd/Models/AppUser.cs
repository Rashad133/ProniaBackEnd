using Microsoft.AspNetCore.Identity;


namespace ProniaBackEnd.Models
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Gender {  get; set; } 
    }
}
