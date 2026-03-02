using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeBudgetManager.Core.DBTables
{
    [Table("repetable_transactions")]
    public class DBRepetableTransaction
    {
        [Key]
        [Column("transaction_id")]
        public int TransactionId { get; set; }

        [Required]
        [Column("value")]
        public decimal Value { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("transaction_title")]
        public required string Title { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public DBCategory? Category { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public DBUser? User { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("next_run_date")]
        public DateTime NextRunDate { get; set; }

        [Column("transaction_interval")]
        public int TransactionInterval { get; set; } = 1;

        [Column("frequency_unit")] 
        public int FrequencyUnit { get; set; } // np. 0=Dni, 1=Tygodnie, 2=Miesiące, 3=Lata

        [ForeignKey(nameof(TransactionId))]
        public virtual DBTransaction? Transaction { get; set; }
    }
}