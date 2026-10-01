using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexoraSuite.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; } = default!;
        [Required, StringLength(150)]
        public string Address { get; set; } = default!;
        [Required, Column(TypeName = "date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BusinessStart { get; set; }
        [Required, StringLength(50)]
        public string Phone { get; set; } = default!;
        [Required, StringLength(50)]
        public string CustomerType { get; set; } = default!;
        [Required, StringLength(50)]
        public string Email { get; set; } = default!;
        [Required, Column(TypeName = "money")]
        public decimal CreditLimit { get; set; }
        public string? Photo { get; set; } = default!;
        //nev
        public virtual ICollection<DeliveryAddress> DeliveryAddresses { get; set; } = new List<DeliveryAddress>();

    }
    public class DeliveryAddress
    {
        public int DeliveryAddressId { get; set; }
        [Required, ForeignKey("Customer")]
        public int CustomerId { get; set; }
        [Required]
        public string ContactPerson { get; set; } = default!;
        [Required, Phone]
        public string Phone { get; set; } = default!;
        public string Address { get; set; } = default!;
        public virtual Customer? Customer { get; set; }

    }
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<DeliveryAddress> DeliveryAddresses { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
