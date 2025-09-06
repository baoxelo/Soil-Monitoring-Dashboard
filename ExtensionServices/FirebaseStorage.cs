using Firebase.Auth;
using Firebase.Storage;
using Google.Cloud.Storage.V1;
using Soil_Monitoring_Web_App.IExtensionServices;
using Soil_Monitoring_Web_App.Models;
namespace Soil_Monitoring_Web_App.ExtensionServices
{
    public class FirebaseStorage : IFirebaseStorage
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly DatabaseContext _context;
        private readonly StorageClient _storageClient;

        //Firebase info
        private readonly string _bucketName = "shoesshop-88775.appspot.com";
        public FirebaseStorage()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "firebaseServiceAccountKey.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            _storageClient = StorageClient.Create();
        }
        public async Task<string> UploadImageFromUrlAsync(string imageUrl, string directory)
        {
            using var httpClient = new HttpClient();
            var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
            using var stream = new MemoryStream(imageBytes);

            string fileName = $"{Guid.NewGuid()}.jpg";
            string objectName = $"{directory}/{fileName}";

            var obj = await _storageClient.UploadObjectAsync(
                bucket: _bucketName,
                objectName: objectName,
                contentType: "image/jpeg",
                source: stream
            );

            // make file public
            await _storageClient.UpdateObjectAsync(obj, new UpdateObjectOptions
            {
                PredefinedAcl = PredefinedObjectAcl.PublicRead
            });

            return $"https://storage.googleapis.com/{_bucketName}/{objectName}";
        }
        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            var objectName = $"images/{Guid.NewGuid()}_{fileName}";

            await _storageClient.UploadObjectAsync(
                bucket: _bucketName,
                objectName: objectName,
                contentType: contentType,
                source: fileStream);

            return $"https://storage.googleapis.com/{_bucketName}/{objectName}";
        }
    }
}
