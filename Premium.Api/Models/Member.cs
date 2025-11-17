using premium.Api.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace premium.Api.Models
{
    public class Member
    {
        [Key]
        public int Id { get; set; }

        [Required] public string Name { get; set; } = null!;
        [Required] public int AgeNextBirthday { get; set; }
        [Required] public string DateOfBirthMMYYYY { get; set; } = null!;
        [Required] public string OccupationCode { get; set; } = null!;
        [ForeignKey(nameof(OccupationCode))] public Occupation? Occupation { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DeathSumInsured { get; set; }
        [Required]
        //[Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyPremium { get; set; }
    }
}
