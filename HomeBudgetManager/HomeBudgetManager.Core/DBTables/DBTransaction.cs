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
        public int transaction_id { get; set; }

        public string transaction_category { get; set; }

        public string transaction_description { get; set; } 

        [Required]
        public decimal transaction_value { get; set; }

        [Column("transaction_from_user_id")]
        public int DBUserId { get; set; }
        public DBUser DBUser { get; set; }

        [Column("transaction_for_house_id")]
        public int DBHouseId { get; set; }
        public DBHouse DBHouse { get; set; }

        [Required]
        [DataType(DataType.DateTime)]       
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime transaction_date { get; set; } = DateTime.Now;

        public bool transaction_is_repetable { get; set; }

    }


}