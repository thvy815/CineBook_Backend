using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricingService.Domain.DTOs
{
    public class G_PROMOTION
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = "";
        public string DiscountType { get; set; } = "";
        public decimal DiscountValue { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; } = "";
    }
}
