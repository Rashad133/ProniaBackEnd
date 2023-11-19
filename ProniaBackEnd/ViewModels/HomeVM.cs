using Microsoft.EntityFrameworkCore;
using ProniaBackEnd.Models;

namespace ProniaBackEnd.ViewModels
{
    public class HomeVM
    {
        public List<Slide> Slides { get; set; }
        public List<Product> Products { get; set; }
    }
}
