using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("payment_transaction")] // bắt buộc phải trùng với tên bảng trong PostgreSQL
public class Payment
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("booking_id")]
    public Guid BookingId { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("method")]
    public string PaymentMethod { get; set; }

    [Column("status")]
    public string Status { get; set; }

    [Column("transaction_ref")]
    public string TransactionId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
