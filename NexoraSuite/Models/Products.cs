using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexoraSuite.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = default!;

        [StringLength(50)]
        public string? Category { get; set; }

        [StringLength(20)]
        public string? SKU { get; set; }

        [Required, Column(TypeName = "money")]
        public decimal Price { get; set; }

        public int Stock { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}