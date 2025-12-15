using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricingService.Domain.DTOs
{
    public class G_SEATPRICEDTO
    {
        public Guid Id { get; set; }
        public string SeatType { get; set; }
        public string TicketType { get; set; }
        public decimal BasePrice { get; set; }
        public string Description { get; set; }
    }
}
