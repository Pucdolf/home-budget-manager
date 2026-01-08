using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgetManager.Core.DBTables
{

    [Table("categories")]
    public class DBCategory
    {
        [Key]
        [Column("category_id")]
        public int Id { get; set; }

        [ForeignKey("user_id")]
        public int? userId {  get; set; } // jezeli userId == null to sa to kategorie domyslne

        [Required]
        [Column("category_name")]
        public required string Name { get; set; }

        [Column("category_description")]
        public string? Description { get; set; }
    }

}
