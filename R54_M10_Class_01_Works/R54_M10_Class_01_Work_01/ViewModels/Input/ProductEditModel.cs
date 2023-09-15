using Microsoft.EntityFrameworkCore.Metadata.Internal;
using R54_M10_Class_01_Work_01.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace R54_M10_Class_01_Work_01.ViewModels.Input
{
    public class ProductEditModel
    {
        public int ProductId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = default!;

        
        public IFormFile? Picture { get; set; } = default!;

        [Required]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }

        [EnumDataType(typeof(SellUnit))]
        public SellUnit SellUnit { get; set; }

        public virtual List<ProductInventory> ProductInventories { get; set; } = new List<ProductInventory>();
    }
}
