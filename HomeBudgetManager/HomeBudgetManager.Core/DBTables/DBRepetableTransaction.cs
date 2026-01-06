using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeBudgetManager.Core.DBTables
{
    [Table("repetable_transactions")]
    public class DBRepetableTransaction
    {
        [Key]
        [Column("repetable_transaction_id")]
        [ForeignKey(nameof(DBTransaction))]
        public required int TransactionId { get; set; }
        public required DBTransaction Transaction { get; set; }


        [Required]
        [Column("repetable_transaction_renew_interval")]
        public required decimal TransactionInterval { get; set; }

    }
}
