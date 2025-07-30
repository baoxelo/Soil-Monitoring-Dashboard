namespace Soil_Monitoring_Web_App.IExtensionServices
{
    public interface ICsvToSqlService
    {
        Task ImportCsvToDatabase();
    }
}
