using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soil_Monitoring_Web_App.Models
{
    [Table("SoilData")]
    public class SoilData
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "FLOAT")]
        public double N { get; set; }

        [Column(TypeName = "FLOAT")]
        public double P { get; set; }

        [Column(TypeName = "FLOAT")]
        public double K { get; set; }

        [Column(TypeName = "DECIMAL(5,2)")]
        public double Humidity { get; set; }

        [Column(TypeName = "decimal(10,4)")]
        public decimal PH { get; set; }

        [Column(TypeName = "DECIMAL(6,3)")]
        public double EC { get; set; }

        [Column(TypeName = "DECIMAL(5,2)")]
        public double Temp { get; set; }

        [ForeignKey(nameof(Sensor))]
        public int SensorId { get; set; }   
        public Sensor? Sensor { get; set; }

        [ForeignKey(nameof(MeasurementDate))]
        public int MeasurementDateId { get; set;}
        public MeasurementDate? MeasurementDate { get; set; }

        


    }
}
