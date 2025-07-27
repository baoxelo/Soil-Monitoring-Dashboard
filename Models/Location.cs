using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soil_Monitoring_Web_App.Models
{
    [Table("Location")]
    public class Location
    {
        [Key]
        public int Id { get; set; }

        [Column]
        public string?  Lattitude { get; set; }

        [Column]
        public string? Longtitude { get; set; }

        [NotMapped]
        public Sensor? Sensor { get; set; }
    }
}
