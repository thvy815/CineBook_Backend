using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricingService.Domain.Entities
{
    [Table("fnb_item")]
    public class FnbItem
    {
        [Key]
        [Column("id")]
        public Guid id { get; set; }

        [Column("name")]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }  // PostgreSQL text type → string

        [Column("unit_price", TypeName = "numeric(10,2)")]
        public decimal UnitPrice { get; set; }
    }
}
