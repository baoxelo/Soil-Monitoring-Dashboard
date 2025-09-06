namespace Soil_Monitoring_Web_App.IExtensionServices
{
    public interface IFirebaseStorage 
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
        Task<string> UploadImageFromUrlAsync(string imageUrl, string directory);
    }
}
