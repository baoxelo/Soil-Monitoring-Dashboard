using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Soil_Monitoring_Web_App.IExtensionServices;
using Soil_Monitoring_Web_App.Models;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Soil_Monitoring_Web_App.ExtensionServices
{
    public class CsvToSqlService : ICsvToSqlService
    {
        private readonly ILogger<CsvToSqlService> _logger;
        private readonly DatabaseContext _context;
        private static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        private static readonly string ApplicationName = "Soil Monitoring Dashboard";
        private static readonly string SheetId = "1sWX-UQAUgE_QDEieL2T__V_3SwvkXBK6jrHuML9Ayf0";
        private static readonly string Range = "Sheet1";
        string outputFile = "Data/data_from_sheet.csv";
        public CsvToSqlService (DatabaseContext context, ILogger<CsvToSqlService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task ImportCsvToDatabase()
        {
            
            // --- Read from credentials.json ---
            GoogleCredential credential;
            using (var stream = new FileStream("GoogleCredential.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            }

            // --- CREATE SERVICE ---
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // --- CALL API ---
            var request = service.Spreadsheets.Values.Get(SheetId, Range);
            var response = await request.ExecuteAsync();
            var values = response.Values;


            // --- SAVE FILE CSV ---
            if (values == null && values.Count == 0)
            {
                _logger.LogWarning("❌ Sheet has no data.");
            }
            var sb = new StringBuilder();

            foreach (var row in values)
            {
                var line = string.Join(",", row.Select(cell => $"\"{cell.ToString().Replace("\"", "\"\"")}\""));
                sb.AppendLine(line);
            }

            File.WriteAllText(outputFile, sb.ToString(), Encoding.UTF8);
            _logger.LogInformation($"✅ Save at: {outputFile}");


            // --- UPDATE DATABASE ---

            if (!_context.Sensors.Any(q => q.Name == 1))
            {
                var sensor = new Sensor()
                {
                    Name = 1,

                };
                await _context.Sensors.AddAsync(sensor);
            }
            var lines = await File.ReadAllLinesAsync(outputFile);
            var soilDataList = new List<SoilData>();

            foreach (var line in lines.Skip(1)) // skip header
            {
                List<string> parts = line.Split(',').ToList();

                if (!parts.Contains("0"))
                {
                    var sensor = _context.Sensors.FirstOrDefault(q => q.Name == 1);
                    var data = new SoilData
                    {
                        N = int.Parse(parts[3]),
                        P = int.Parse(parts[4]),
                        K = int.Parse(parts[5]),
                        Humidity = float.Parse(parts[6]),
                        PH = int.Parse(parts[7]),
                        EC = int.Parse(parts[8]),
                        Temp = float.Parse(parts[9]),
                        Date = DateTime.Parse(parts[1]),
                        Time = TimeSpan.Parse(parts[2]),
                        SensorId = sensor?.Id ?? 0,
                    };
                    soilDataList.Add(data);

                    if (sensor.Latitude != parts[10] || sensor.Longtitude != parts[11])
                    {
                        sensor.Latitude = parts[10];
                        sensor.Longtitude = parts[11];
                        await _context.SaveChangesAsync();
                    }


                }



            }
            
            _context.AddRange(soilDataList);
            await _context.SaveChangesAsync();


        }
    }
}
