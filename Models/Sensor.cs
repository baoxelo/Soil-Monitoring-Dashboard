using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soil_Monitoring_Web_App.Models
{
    public class Sensor
    {
        [Key]
        public int Id { get; set; }

        public int Name { get; set; }

        public string? Longtitude { get; set; }

        public string? Latitude { get; set; }


        [NotMapped]
        public List<SoilData>? SoilDatas { get; set; }
    }
}
