using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace HomeBudgetManager.Core.DBTables
{
    [Index(nameof(Name), IsUnique = true)] //unikalna nazwa domu
    [Table("houses")]
    public class DBHouse
    {
        [Key]
        [Column("house_id")]
        public int HouseId { get; set; }

        [Required]
        [Column("house_admin_id")]
        public int DBUserId { get; set; }

        [ForeignKey("DBUserId")]
        public DBUser DBUser { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }  // opis (opcjonalny)

        public ICollection<DBUser> Members { get; set; } = new List<DBUser>();
    }
}
