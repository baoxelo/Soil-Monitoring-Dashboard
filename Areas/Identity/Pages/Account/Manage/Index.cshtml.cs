using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Soil_Monitoring_Web_App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using Soil_Monitoring_Web_App.ExtensionServices;

namespace Soil_Monitoring_Web_App.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly FirebaseStorage _firebaseController;

        public IndexModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            FirebaseStorage firebaseController)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _firebaseController = firebaseController;
        }
        public string Avatar { set; get; }
        public string UserName { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Tên đầy đủ")]
            public string FullName { get; set; }

            [Phone]
            [Display(Name = "Số điện thoại")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Địa chỉ")]
            public string Address { get; set; }

            public Stream? ImageFile { get; set; }
        }

        private async Task LoadAsync(AppUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            UserName = userName;
            Avatar = user.Avatar;

            Input = new InputModel
            {
                FullName = user.FullName,
                PhoneNumber = phoneNumber,
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            user.FullName = Input.FullName;
            if(Input.ImageFile != null) 
            {
                var fileName = user.Email;
                var imageLink = await _firebaseController.UploadFileAsync(Input.ImageFile, fileName, "Users");
                user.Avatar = imageLink;
            }
            var result = await _userManager.UpdateAsync(user);
            if(!result.Succeeded)
            {
                StatusMessage = "Tên đầy đủ và địa chỉ có lỗi";
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Thông tin cá nhân đã được cập nhật";
            return RedirectToPage();
        }
    }
}
