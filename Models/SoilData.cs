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

        [Column(TypeName = "Decimal(5,2)")]
        public double PH { get; set; }

        [Column(TypeName = "DECIMAL(10,4)")]
        public double EC { get; set; }

        [Column(TypeName = "DECIMAL(5,2)")]
        public double Moisture { get; set; }

        [Column(TypeName = "DECIMAL(5,2)")]
        public double Temp { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan Time { get; set; }

        [ForeignKey(nameof(Sensor))]
        public int SensorId { get; set; }   
        public Sensor? Sensor { get; set; }

        

        


    }
}
