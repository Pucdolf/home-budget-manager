using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// TODO: finish all tables

namespace HomeBudgetManager.Core.DBTables
{
    [Table("houses")]
    public class DBHouse
    {
        [Key]
        public int house_id { get; set; }

        [Required]
        [Column("house_admin_id")]
        public int DBUserId { get; set; }
        [ForeignKey("DBUserId")]
        public DBUser DBUser { get; set; }


    }


}