using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricingService.Domain.DTOs
{
    public class PromotionDTOs
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = "";
        public string DiscountType { get; set; } = "";
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsOneTimeUse { get; set; }
        public string Description { get; set; } = "";
    }

    public class UpdatePromotionRequest
    {
        public string DiscountType { get; set; } = null!;
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsOneTimeUse { get; set; }
        public string? Description { get; set; }
    }
}
