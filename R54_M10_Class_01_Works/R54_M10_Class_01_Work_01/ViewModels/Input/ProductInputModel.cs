using Microsoft.EntityFrameworkCore.Metadata.Internal;
using R54_M10_Class_01_Work_01.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace R54_M10_Class_01_Work_01.ViewModels.Input
{
    public class ProductInputModel
    {
        public int ProductId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = default!;

        [Required]
        public IFormFile Picture { get; set; } = default!;

        [Required]
        [DataType(DataType.Currency), Display(Name ="Unit price")]
        public decimal? UnitPrice { get; set; }

        [EnumDataType(typeof(SellUnit)), Display(Name ="Sold by")]
        public SellUnit SellUnit { get; set; }

        public virtual List<ProductInventory> ProductInventories { get; set; } = new List<ProductInventory>();
    }
}
