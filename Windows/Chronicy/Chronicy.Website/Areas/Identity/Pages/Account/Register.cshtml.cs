using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Chronicy.Website.Identity;
using Chronicy.Website.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Chronicy.Website.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public InputData Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputData
        {
            [Required]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public RegisterModel(UserManager<ChronicyUser> userManager,
                             //SignInManager<ChronicyUser> signInManager,
                             ILogger<RegisterModel> logger,
                             IEmailSender emailSender,
                             IEmailBuilder emailBuilder)
        {
            this.userManager = userManager;
            //this.signInManager = signInManager;
            this.logger = logger;

            this.emailSender = emailSender;
            this.emailBuilder = emailBuilder;
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (returnUrl == null)
            {
                returnUrl = Url.Content("~/");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            ChronicyUser user = new ChronicyUser { UserName = Input.Username, Email = Input.Email };
            IdentityResult result = await userManager.CreateAsync(user, Input.Password);

            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return Page();
            }

            logger.LogInformation("User created a new account with password.");

            string token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            string callbackUrl = Url.Page(
                WebsitePaths.ConfirmEmail,
                pageHandler: null,
                values: new { userId = user.Id, token },
                protocol: Request.Scheme);

            await emailSender.SendEmailAsync(Input.Email, "Confirm your email", emailBuilder.Build(callbackUrl));

            return LocalRedirect(returnUrl);
        }

        private readonly UserManager<ChronicyUser> userManager;
        //private readonly SignInManager<ChronicyUser> signInManager;
        private readonly ILogger<RegisterModel> logger;

        private readonly IEmailSender emailSender;
        private readonly IEmailBuilder emailBuilder;
    }
}