using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricingService.Domain.Entities
{
    [Table("promotion")]
    public class Promotion
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("code")]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Column("discount_type")]
        [MaxLength(20)]
        public string DiscountType { get; set; } = string.Empty;

        [Column("discount_value", TypeName = "numeric(10,2)")]
        public decimal DiscountValue { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("is_one_time_use")]
        public bool IsOneTimeUse { get; set; }

        [Column("description")]
        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
