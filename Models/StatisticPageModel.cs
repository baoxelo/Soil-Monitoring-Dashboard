
namespace Soil_Monitoring_Web_App.Models
{
    public class StatisticData
    {
        public double Data { get; set; }
        public string Date { get; set; }
    }
    public class StatisticPageModel
    {
        public List<StatisticData> N {  get; set; }
        public List<StatisticData> P {  get; set; }
        public List<StatisticData> K {  get; set; }
        public List<StatisticData> Humiditty {  get; set; }
        public List<StatisticData> Temp {  get; set; }
        public List<StatisticData> pH {  get; set; }
        public List<StatisticData> EC {  get; set; }
    }
}
