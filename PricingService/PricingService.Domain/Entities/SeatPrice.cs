using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricingService.Domain.Entities
{
    [Table("seat_price")]
    public class SeatPrice
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("seat_type")]
        [MaxLength(50)]
        public string SeatType { get; set; } = string.Empty;

        [Column("ticket_type")]
        [MaxLength(50)]
        public string TicketType { get; set; } = string.Empty;

        [Column("base_price", TypeName = "numeric(10,2)")]
        public decimal BasePrice { get; set; }

        [Column("description")]
        [MaxLength(255)]
        public string? Description { get; set; }
    }

}
