using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Soil_Monitoring_Web_App.Models
{
    public class AppUser : IdentityUser
    {
        [Required(ErrorMessage = "Required {0}")]
        [DisplayName("FullName")]
        [StringLength(50, MinimumLength = 3 )]
        [Column(TypeName = "nvarchar(50)")]
        public required string FullName { get; set; }

        [DisplayName("Avatar")]
        [Column(TypeName = "nvarchar(500)")]
        public string? Avatar { get; set; }

        [DisplayName("BirthDate")]
        [Column(TypeName = "date")]
        public DateOnly? BirthDate { get; set; }


        [NotMapped]
        public string? RoleNames { get; set; }
    }
}
