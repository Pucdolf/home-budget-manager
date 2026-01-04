using System;
using System.Collections.Generic;
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
        public int categoryId { get; set; }

        [Required]
        [Column("category_name")]
        public string categoryName { get; set; }

        [Column("category_description")]
        public string? categoryDescription { get; set; }
    }

}
