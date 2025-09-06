using CsvHelper;
using CsvHelper.Configuration;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Soil_Monitoring_Web_App.IExtensionServices;
using Soil_Monitoring_Web_App.Models;
using System.Globalization;
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
                _logger.LogInformation("Create sensor");
                var sensor = new Sensor()
                {
                    Name = 1,

                };
                await _context.Sensors.AddAsync(sensor);
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation("Insert to database");
            var soilDataList = new List<SoilData>();
            using var reader = new StreamReader(outputFile);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Read();
            csv.ReadHeader();


            while (csv.Read())
            {
                var record = csv.Parser.Record; // string[] of 1 line

                if (!record.Contains("0") && !_context.SoilDatas.Any(q => q.Time == TimeSpan.Parse(csv.GetField<string>(1)))   )
                {
                    var sensor = _context.Sensors.FirstOrDefault(q => q.Name == 1);
                    var data = new SoilData
                    {
                        N = csv.GetField<int>("N"),
                        P = csv.GetField<int>("P"),
                        K = csv.GetField<int>("K"),
                        Humidity = csv.GetField<float>("Humiditty"),
                        PH = csv.GetField<float>("Ph"),
                        EC = csv.GetField<float>("EC")/1000,
                        Temp = csv.GetField<float>("Temp"),
                        Date = DateTime.Parse(csv.GetField<string>("Date")),  
                        Time = TimeSpan.Parse(csv.GetField<string>("Time")),  
                        SensorId = sensor?.Id ?? 0,
                    };

                    soilDataList.Add(data);

                    if (sensor.Latitude != csv.GetField<string>("Lattitude") || sensor.Longitude != csv.GetField<string>("Longtitude"))
                    {
                        sensor.Latitude = csv.GetField<string>("Lattitude");
                        sensor.Longitude = csv.GetField<string>("Longtitude");
                        await _context.SaveChangesAsync();
                    }
                }
            }
            await _context.SoilDatas.AddRangeAsync(soilDataList);
            await _context.SaveChangesAsync();
        }
    }
}
