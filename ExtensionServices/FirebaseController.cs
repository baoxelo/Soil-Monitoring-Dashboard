using Firebase.Auth;
using Firebase.Storage;
using Soil_Monitoring_Web_App.Models;
namespace Soil_Monitoring_Web_App.ExtensionServices
{
    public class FirebaseController
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly DatabaseContext _context;

        //Firebase info
        private static string ApiKey = "AIzaSyDwQeHRUG5BfmFQoR18Gd0v2ZGRKlPPHwI";
        private static string Bucket = "shoesshop-88775.appspot.com";
        private static string AuthEmail = "skyyourmtp@gmail.com";
        private static string AuthPassword = "baobao2001";

        public FirebaseController( DatabaseContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }
        public async Task<string> UploadImagetoFirebase(IFormFile file, string fileName, string directory)
        {
            string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/Images");
            if (!Directory.Exists(directoryPath))
            {
                // Nếu thư mục chưa tồn tại, tạo thư mục mới
                Directory.CreateDirectory(directoryPath);
            }
            FileStream ? stream = null;
            using (stream = new FileStream(Path.Combine("Data/Images/", fileName), FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            stream = new FileStream(Path.Combine("Data/Images/", fileName), FileMode.Open);
            // of course you can login using other method, not just email+password
            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

            // you can use CancellationTokenSource to cancel the upload midway
            var cancellation = new CancellationTokenSource();
            var task = new FirebaseStorage(
                    Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
                    })
                    .Child("images")
                    .Child(directory)
                    .Child(fileName)
                    .PutAsync(stream, cancellation.Token);

            try
            {
                string link = await task;
                stream.Close();
                File.Delete(Path.Combine("Data/Images/", fileName));
                return link;
            }
            catch (Exception ex)
            {
                string link = Path.Combine("Data/Images/", fileName).ToString();
                Console.WriteLine("Exception was thrown: {0}", ex);
                return link;
            }
        }

        public async Task DeleteImagetoFirebase(string fileName, string directory)
        {
            string path = Path.Combine(directory, fileName);
            // of course you can login using other method, not just email+password
            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

            // you can use CancellationTokenSource to cancel the upload midway
            var cancellation = new CancellationTokenSource();
            var task = new FirebaseStorage(
                    Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
                    })
                    .Child("images")
                    .Child(path)
                    .DeleteAsync();

            try
            {
                await task;
            }
            catch (Exception ex)
            {
                string link = Path.Combine("Data/Images/", fileName).ToString();
                Console.WriteLine("Exception was thrown: {0}", ex);
            }
        }
    }
}
