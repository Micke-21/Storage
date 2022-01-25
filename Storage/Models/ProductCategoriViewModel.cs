namespace Storage.Models
{
    public class ProductCategoriViewModel
    {
        public IEnumerable<string>? Categories { get; set; }
        public IEnumerable<ProductViewModel>? Products { get; set; }
    }
}
