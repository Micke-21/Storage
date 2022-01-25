using System.ComponentModel.DataAnnotations;

namespace Storage.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Range(0,1000000, ErrorMessage ="Vär det {0} måste vara mellan {1} och {2}")]
        public int Price { get; set; }

        [DataType(DataType.Date)]
        public DateTime Orderdate { get; set; }

        [Required]
        public string Category { get; set; }

        [MaxLength (10)]
        public string Shelf { get; set; }

        [Range(0, maximum: int.MaxValue)]
        public int Count { get; set; }

        public string? Description { get; set; }

    }
}
