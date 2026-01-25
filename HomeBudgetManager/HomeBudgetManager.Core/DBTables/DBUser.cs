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

    [Table("roles")]
    public class DBRole
    {
        // Używamy DatabaseGeneratedOption.None, bo chcemy, aby ID w bazie
        // dokładnie odpowiadało wartościom z Enuma (0, 1, 2...), a nie było autoincrement
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("role_id")]
        public SystemRole Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("role_name")]
        public required string Name { get; set; }
    }

    [Index(nameof(Email), IsUnique = true)]
    [Table("users")]
    public class DBUser
    {
        [Key]
        [Column("user_id")]
        public int Id { get; set; }

        [Required]
        [Column("user_email")]
        public required string Email { get; set; }

        [Required]
        [Column("user_login")]
        public required string Login { get; set; }

        [Required]
        [Column("user_password")]
        public required string Password { get; set; }

        [Column("user_role")]
        public SystemRole Role { get; set; } = SystemRole.Guest;

        [Column("user_house_id")]
        public int? HouseId { get; set; }

        [ForeignKey(nameof(HouseId))]
        public DBHouse? House { get; set; }

        public ICollection<DBCategory> Categories { get; set; } = new List<DBCategory>();
        public ICollection<DBTransaction> Transactions { get; set; } = new List<DBTransaction>();
    }
}