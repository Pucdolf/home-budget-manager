using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// TODO: finish all tables

namespace HomeBudgetManager.Core.DBTables
{
    [Index(nameof(user_email), IsUnique = true)]
    [Table("users")]
    public class DBUser
    {
        [Key]
        public int user_id { get; set; }

        [Required]
        public string user_email { get; set; }

        [Required]
        public string user_password { get; set; }

        [Column("user_house_id")]
        public int DBHouseId { get; set; }
        [ForeignKey("DBHouseId")]
        public DBHouse DBHouse { get; set; }

    }

    
}