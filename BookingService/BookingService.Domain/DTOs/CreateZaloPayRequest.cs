using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingService.Domain.DTOs
{
    public class CreateZaloPayOrderRequest { 
        public Guid BookingId { get; set; } 
        public Guid UserId { get; set; } 
        public decimal Amount { get; set; } }
}


