using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Soil_Monitoring_Web_App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Soil_Monitoring_Web_App.IExtensionServices;

namespace Soil_Monitoring_Web_App.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ExternalLoginModel> _logger;
        private readonly IFirebaseStorage _firebaseStorage;

        public ExternalLoginModel(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            ILogger<ExternalLoginModel> logger,
            IFirebaseStorage firebaseStorage
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _firebaseStorage = firebaseStorage;

        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ProviderDisplayName { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            public string Email { get; set; }


            [Required]
            [Phone]
            public string PhoneNumber { get; set; }
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor : true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                ProviderDisplayName = info.ProviderDisplayName;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };
                }
                return Page();
            }
        }
        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error !!";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (!ModelState.IsValid)
            {
                ProviderDisplayName = info.ProviderDisplayName;
                ReturnUrl = returnUrl;
                return Page();
            }
            _logger.LogInformation("Kiểm tra phone 123");
            // Kiểm tra số điện thoại đã đăng ký
            var registedPhoneNumber = await _userManager.FindByNameAsync(Input.PhoneNumber);
            if (registedPhoneNumber != null)
            {
                ErrorMessage = "This phone number is already registered. Please use another one.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Kiểm tra email đã tồn tại
            var registedUser = await _userManager.FindByEmailAsync(Input.Email);

            string externalEmail = info.Principal.FindFirstValue(ClaimTypes.Email);
            AppUser externalEmailUser = null;

            if (!string.IsNullOrEmpty(externalEmail))
                externalEmailUser = await _userManager.FindByEmailAsync(externalEmail);

            // Nếu email external đã tồn tại và khớp với user nhập
            if (externalEmailUser != null && registedUser != null && registedUser.Id == externalEmailUser.Id)
            {
                var resultLink = await _userManager.AddLoginAsync(registedUser, info);
                if (resultLink.Succeeded)
                {
                    await _signInManager.SignInAsync(registedUser, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
            }

            // Nếu email external tồn tại nhưng user nhập không khớp
            if (externalEmailUser != null && registedUser == null)
            {
                ModelState.AddModelError(string.Empty, "Cannot link account. Please use a different email.");
                return Page();
            }

            // Tạo user mới
            var user = new AppUser
            {
                FullName = info.Principal.Identity.Name,
                UserName = Input.PhoneNumber,
                Email = externalEmail,
                Avatar = await _firebaseStorage.UploadImageFromUrlAsync(info.Principal.FindFirstValue("image"), Input.PhoneNumber),
                EmailConfirmed = true // Không cần confirm email
            };

            var newPassword = GenerateRandomPassword();
            user.PasswordHash = new PasswordHasher<AppUser>().HashPassword(user, newPassword);

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                var loginResult = await _userManager.AddLoginAsync(user, info);
                if (loginResult.Succeeded)
                {
                    _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                    await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);

                    return RedirectToAction("Index", "Home");
                }
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;
            return Page();
        }
        private string GenerateRandomPassword()
        {
            // Đây là một cách đơn giản, bạn có thể thay đổi phương thức tạo mật khẩu tùy thuộc vào nhu cầu của bạn
            var options = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var password = new string(Enumerable.Repeat(options, 8).Select(s => s[random.Next(s.Length)]).ToArray());
            return password;
        }
    }
}
