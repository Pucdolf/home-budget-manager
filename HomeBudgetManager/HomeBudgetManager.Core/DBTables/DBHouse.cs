using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace HomeBudgetManager.Core.DBTables
{
    [Table("houses")]
    public class DBHouse
    {
        [Key]
        [Column("house_id")]
        public int Id { get; set; }

        [Required]
        [Column("house_admin_id")]
        public int AdminId { get; set; }

        [ForeignKey(nameof(AdminId))]
        public DBUser Admin { get; set; }

        [Required]
        [Column("house_name")]
        public required string Name { get; set; }

        [Column("house_description")]
        public string? Description { get; set; }  // opis (opcjonalny)

        [Required]
        [Column("house_join_code")]
        public required string JoinCode { get; set; }

        public ICollection<DBUser> Members { get; set; } = new List<DBUser>();
    }
}
