using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace HomeBudgetManager.Core.DBTables
{
    [Index(nameof(house_name), IsUnique = true)] //unikalna nazwa domu
    [Table("houses")]
    public class DBHouse
    {
        [Key]
        [Column("house_id")]
        public int house_id { get; set; }

        [Required]
        [Column("house_admin_id")]
        public int house_admin_id { get; set; }

        [ForeignKey("house_admin_id")]
        public DBUser admin { get; set; }

        [Required]
        [Column("house_name")]
        public string house_name { get; set; }

        [Column("house_description")]
        public string? house_description { get; set; }  // opis (opcjonalny)

        public ICollection<DBUser> Members { get; set; } = new List<DBUser>();
    }
}
