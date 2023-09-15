using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace R54_M10_Class_01_Work_01.Models
{
    public enum SellUnit { kg = 1, g, mg, l, ml, nos }
    public class Product
    {
        public int ProductId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = default!;

        [Required, StringLength(100)]
        public string Picture { get; set; } = default!;

        [Required]
        [Column(TypeName = "money")]
        public decimal UnitPrice { get; set; }

        [EnumDataType(typeof(SellUnit))]
        public SellUnit SellUnit { get; set; }

        public virtual ICollection<ProductInventory> ProductInventories { get; set; } = new List<ProductInventory>();
    }
    public class ProductInventory
    {
        public int ProductInventoryId { get; set; }

        [Required, Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [Required]
        public int? Quantity { get; set; }

        [Required, ForeignKey("Product")]
        public int ProductId { get; set; }

        public virtual Product? Product { get; set; } = default!;
    }
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<ProductInventory> ProductInventories { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            for (int i = 1; i <= 7; i++)
            {
                Product p1 = new Product { ProductId = i, Name = $"P{i:00}", UnitPrice = i * 950.00M, SellUnit = SellUnit.kg, Picture = $"{i}.jpg" };
                modelBuilder.Entity<Product>().HasData(p1);
            }
            for (int i = 1; i <= 7; i++)
            {

                ProductInventory pi = new ProductInventory { ProductInventoryId = i, Date = DateTime.Now.AddDays(-23 * i), Quantity = 50 * i, ProductId = i };

                modelBuilder.Entity<ProductInventory>().HasData(pi);


            }
            for (int i = 8; i <= 10; i++)
            {
                ProductInventory pi = new ProductInventory { ProductInventoryId = i, Date = DateTime.Now.AddDays(-23 * i), Quantity = 50 * i, ProductId = i - 7 };

                modelBuilder.Entity<ProductInventory>().HasData(pi);


            }
            //modelBuilder.Entity<Product>().HasData(
            //    new Product { ProductId=1, Name="P1", UnitPrice=900.00M, SellUnit=SellUnit.kg, Picture="1.jpg"}
            //    );
            //modelBuilder.Entity<ProductInventory>().HasData(
            //    new ProductInventory { ProductInventoryId = 1, Date=DateTime.Now.AddDays(-10), Quantity=100, ProductId=1 }
            //    );
        }
    }
}
