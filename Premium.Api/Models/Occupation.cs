using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace premium.Api.Models
{
    public class Occupation
    {
        [Key]
       // public int Id { get; set; }
        public string Code { get; set; } = null!; // e.g. "Doctor"
        public string DisplayName { get; set; } = null!;
        public string Rating { get; set; } = null!;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Factor { get; set; } 
    }
}
