// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using DidUFall4It_DDACGroupAssignment_Group21.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

namespace DidUFall4It_DDACGroupAssignment_Group21.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<DidUFall4It_DDACGroupAssignment_Group21User> _signInManager;
        private readonly UserManager<DidUFall4It_DDACGroupAssignment_Group21User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<DidUFall4It_DDACGroupAssignment_Group21User> _userStore;
        private readonly IUserEmailStore<DidUFall4It_DDACGroupAssignment_Group21User> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<DidUFall4It_DDACGroupAssignment_Group21User> userManager,
            IUserStore<DidUFall4It_DDACGroupAssignment_Group21User> userStore,
            SignInManager<DidUFall4It_DDACGroupAssignment_Group21User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }
        public SelectList RoleSelectList = new SelectList(
         new List<SelectListItem>
         {
         new SelectListItem { Selected =true, Text = "Select Role", Value = ""},
         new SelectListItem { Selected =true, Text = "QuizMaker", Value = "QuizMaker"},
         new SelectListItem { Selected =true, Text = "Customer", Value = "Customer"},
         new SelectListItem { Selected =true, Text = "Infographic", Value = "Infographic"},
         }, "Value", "Text",1
         );
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            [Required(ErrorMessage = "You must enter the name first before submitting your form!")]
            [StringLength(256, ErrorMessage = "You must enter the value between 6 - 256 chars", MinimumLength = 6)]
            [Display(Name = "Customer Full Name")] //label
            public string CustomerFullName { get; set; }
            [Required]
            [Display(Name = "Customer DOB")]
            [DataType(DataType.Date)]
            public DateTime DoB { get; set; }

            [Display(Name = "User Role")]
            public string userrole { set; get; }

        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.UserRole = Input.userrole;
                user.CustomerFullName = Input.CustomerFullName;
                user.CustomerDOB = Input.DoB;
                user.EmailConfirmed = true;
                user.UserRole = Input.userrole;
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    bool roleresult = await _roleManager.RoleExistsAsync("Infographic");
                    if (!roleresult)
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Infographic"));
                    }
                    roleresult = await _roleManager.RoleExistsAsync("User");
                    if (!roleresult)
                    {
                        await _roleManager.CreateAsync(new IdentityRole("User"));
                    }
                    roleresult = await _roleManager.RoleExistsAsync("QuizMaker");
                    if (!roleresult)
                    {
                        await _roleManager.CreateAsync(new IdentityRole("QuizMaker"));
                    }
                    await _userManager.AddToRoleAsync(user, Input.userrole);
                    //_logger.LogInformation("User created a new account with password.");

                    //var userId = await _userManager.GetUserIdAsync(user);
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        //return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        return RedirectToPage("Login");
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private DidUFall4It_DDACGroupAssignment_Group21User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<DidUFall4It_DDACGroupAssignment_Group21User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(DidUFall4It_DDACGroupAssignment_Group21User)}'. " +
                    $"Ensure that '{nameof(DidUFall4It_DDACGroupAssignment_Group21User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<DidUFall4It_DDACGroupAssignment_Group21User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<DidUFall4It_DDACGroupAssignment_Group21User>)_userStore;
        }
    }
}
