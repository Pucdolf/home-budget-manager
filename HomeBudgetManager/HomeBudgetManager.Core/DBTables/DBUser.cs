using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeBudgetManager.Core.DBTables
{
    public enum SystemRole
    {
        Guest = 0,            // Nowo zarejestrowany, nie należy do żadnej grupy
        Individual = 1,       // Korzysta bez grupy (tryb indywidualny)
        HouseholdAdmin = 2,   // Twórca domu, zarządza członkami
        HouseholdMember = 3,  // Zwykły członek domu
        SystemAdmin = 4       // Globalny administrator systemu (zarządza aplikacją)
    }

    [Index(nameof(user_email), IsUnique = true)]
    [Table("users")]
    public class DBUser
    {
        [Key]
        [Column("user_id")]
        public int user_id { get; set; }

        [Required]
        [Column("user_email")]
        public string user_email { get; set; }

        [Required]
        [Column("user_login")]
        public string user_login { get; set; }

        [Required]
        [Column("user_password")]
        public string user_password { get; set; }

        [Column("user_role")]
        public SystemRole user_role { get; set; } = SystemRole.Guest;

        [Column("user_house_id")]
        public int? user_house_id { get; set; }

        [ForeignKey("user_house_id")]
        public DBHouse? user_house { get; set; }
    }


}