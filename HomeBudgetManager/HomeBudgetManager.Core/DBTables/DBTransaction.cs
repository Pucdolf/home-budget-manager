using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// TODO: finish all tables

namespace HomeBudgetManager.Core.DBTables
{
    public enum TransactionType
    {
        expense = 0,
        income = 1
    }

    [Table("transactions")]
    public class DBTransaction
    {
        [Key]
        [Column("transaction_id")]
        public int Id { get; set; }

        [Column("category_id")]
        public required int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public DBCategory? Category { get; set; }

        [Column("user_id")]
        public required int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public DBUser? User { get; set; }

        [Required]
        [Column("transaction_value")]
        public decimal Value { get; set; }

        [Required]
        [Column("transaction_type")]
        public TransactionType TransactionType { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("transaction_title")]
        public required string Title { get; set; }

        [Column("transaction_description")]
        public string? Description { get; set; }

        [Column("transaction_for_house_id")] // Czy potrzebne? można to pobrać od użytkownika
        public int? HouseId { get; set; }
        public DBHouse? House { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Column("transaction_date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Column("transaction_is_repeatable")]
        public bool IsRepeatable { get; set; }

        public virtual DBRepetableTransaction? RepetableTransaction { get; set; }
    }
}