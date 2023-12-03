namespace ProniaBackEnd.ViewModels
{
    public class HeaderVM
    {
        public Dictionary<string,string> Settings { get; set; }
        public List<BasketItemVM> Items { get; set; }
    }
}
