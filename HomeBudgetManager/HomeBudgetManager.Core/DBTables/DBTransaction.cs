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

        [Column("category_id")] // To definiuje nazwę w bazie
        public required int CategoryId { get; set; }

        [ForeignKey("CategoryId")] // To definiuje relację C# (opcjonalne przy konwencji nazewniczej)
        public DBCategory? Category { get; set; }

        [ForeignKey("user_id")]
        public required int UserId { get; set; }
        public DBUser? User { get; set; }

        [Required]
        [Column("transaction_value")]
        public decimal Value { get; set; }

        [Required]
        [Column("transaction_type")]
        public TransactionType TransactionType { get; set; }

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