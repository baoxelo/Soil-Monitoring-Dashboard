using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soil_Monitoring_Web_App.Models
{
    [Table("MeasurementDate")]
    public class MeasurementDate
    {
        [Key]
        public int Id { get; set; }

        [Column]
        public DateOnly Date {  get; set; }

        [Column]
        public TimeOnly Time { get; set; }

        [ForeignKey(nameof(Sensor))]
        public int SensorId { get; set; }
        public Sensor? Sensor { get; set; }

        [NotMapped]
        public SoilData? Datas { get; set; }
    }
}
