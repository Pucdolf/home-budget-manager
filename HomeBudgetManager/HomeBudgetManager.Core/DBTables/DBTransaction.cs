using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// TODO: finish all tables

namespace HomeBudgetManager.Core.DBTables
{
    
    [Table("transactions")]
    public class DBTransaction
    {
        [Key]
        [Column("transaction_id")]
        public int Id { get; set; }

        [ForeignKey("category_id")]
        public required int CategoryId { get; set; }
        public DBCategory? Category { get; set; }

        [ForeignKey("user_id")]
        public required int UserId { get; set; }
        public DBUser? User { get; set; }

        [Required]
        [Column("transaction_value")]
        public decimal Value { get; set; }


        [Column("transaction_description")]
        public string? Description { get; set; }

        [Column("transaction_for_house_id")] // Czy potrzebne? można to pobrać od użytkownika
        public int? HouseId { get; set; }
        public DBHouse? House { get; set; } // potrzebne do foreign key

        [Required]
        [DataType(DataType.DateTime)]
        [Column("transaction_date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Column("transaction_is_repeatable")]
        public bool IsRepeatable { get; set; }

        public virtual DBRepetableTransaction? RepetableTransaction { get; set; }
    }
}