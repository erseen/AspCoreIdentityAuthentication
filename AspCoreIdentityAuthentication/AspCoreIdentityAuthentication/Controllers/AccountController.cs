using AspCoreIdentityAuthentication.Identity;
using AspCoreIdentityAuthentication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AspCoreIdentityAuthentication.EmailService;
using System.Net.Mail;
using Newtonsoft.Json.Linq;

namespace AspCoreIdentityAuthentication.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private  IEmailSender _emailSender; 
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;

        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task <IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // generate token
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail", "Account", new
                {
                    userId = user.Id,
                    token = code
                });

                // email
                await _emailSender.SendEmailAsync(model.Email, "Hesabınızı onaylayınız.", $"Lütfen email hesabınızı onaylamak için linke <a href='https://localhost:7208{url}'>tıklayınız.</a>");
                return RedirectToAction("Login", "Account");
            }

            ModelState.AddModelError("", "Bilinmeyen hata oldu lütfen tekrar deneyiniz.");
            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            ViewBag.Message = "";
            if (userId == null || token == null)
            {
                ViewBag.Message = "danger"; 
               
                return View();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    ViewBag.Message = "success";
                    return View();
                }
            }
            ViewBag.Message = "danger";
            return View();
        }
        
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);

            }

            // var user = await _userManager.FindByNameAsync(model.UserName);
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Bu kullanıcı adı ile daha önce hesap oluşturulmamış");
                return View(model);
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Lütfen email hesabınıza gelen link ile üyeliğinizi onaylayınız.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Girilen kullanıcı adı veya parola yanlış");
            return View(model);
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public  async Task<IActionResult> ForgotPassword(string Email)
        {
            ViewBag.Message = "";
          
            if (string.IsNullOrEmpty(Email))
            {
                ViewBag.Message = "danger";
                return View();

            }

            var user = await _userManager.FindByEmailAsync(Email);

            if (user == null)
            {
                return View();
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            ViewBag.Message = "success";
            var url = Url.Action("ResetPassword", "Account", new
            {
                userId = user.Id,
                token = code
            });
            ViewBag.Message = "success";
            // email
            await _emailSender.SendEmailAsync(Email, "Reset Password", $"Parolanızı yenilemek için linke <a href='https://localhost:7208{url}'>tıklayınız.</a>");

            return View();
        }

        public IActionResult ResetPassword(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Home", "Index");
            }

            var model = new ResetPasswordModel { Token = token };

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("Home", "Index");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {
                // Şifre sıfırlama işlemi başarılı
                return RedirectToAction("Login", "Account");
            }

            // Şifre sıfırlama işlemi başarısız
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
                await _signInManager.SignOutAsync();
                return Redirect("~/");
            
        }

    }
}
