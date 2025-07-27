using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soil_Monitoring_Web_App.Models
{
    public class Sensor
    {
        [Key]
        public int Id { get; set; }
        public int Name { get; set; }

        [ForeignKey(nameof(Location))]
        public int LocationId { get; set; }
        public Location? Location { get; set; }

        [NotMapped]
        public List<SoilData>? SoilDatas { get; set; }

        [NotMapped]
        public List<MeasurementDate>? MeasurementDates { get; set; }
    }
}
